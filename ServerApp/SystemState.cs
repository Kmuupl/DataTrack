using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public decimal Balance { get; set; }

    public User(string username, string password, decimal balance)
    {
        Username = username;
        Password = password;
        Balance = balance;
    }
}

class SystemState
{
    private readonly object _lock = new object();
    private const string FilePath = "users.json";
    private Dictionary<string, User> _users = new();

    public SystemState()
    {
        _users = LoadFromFile();
    }

    private Dictionary<string, User> LoadFromFile()
    {
        if (!File.Exists(FilePath))
        {
            return new Dictionary<string, User>
            {
                { "admin", new User("admin", PasswordHelper.HashPassword("1234"), 1000m) },
                { "user", new User("user", PasswordHelper.HashPassword("pass"), 500m) }
            };
        }

        string json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<Dictionary<string, User>>(json) ?? new Dictionary<string, User>();
    }

    private void SaveToFile()
    {
        string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    public bool ValidateUser(string username, string password)
    {
        lock (_lock)
        {
            return _users.TryGetValue(username, out var user) && user.Password == PasswordHelper.HashPassword(password);
        }
    }

    public decimal GetBalance(string username)
    {
        lock (_lock)
        {
            return _users.TryGetValue(username, out var u) ? u.Balance : -1;
        }
    }

    public bool Deposit(string username, decimal amount)
    {
        lock (_lock)
        {
            if (!_users.TryGetValue(username, out var u)) return false;
            u.Balance += amount;
            SaveToFile();
            return true;
        }
    }

    public bool Withdraw(string username, decimal amount)
    {
        lock (_lock)
        {
            if (!_users.TryGetValue(username, out var u) || u.Balance < amount) return false;
            u.Balance -= amount;
            SaveToFile();
            return true;
        }
    }

    public bool UserExists(string username)
    {
        lock (_lock)
        {
            return _users.ContainsKey(username);
        }
    }

    public bool Transfer(string fromUser, string toUser, decimal amount)
    {
        lock (_lock)
        {
            if (!_users.TryGetValue(fromUser, out var from) || !_users.TryGetValue(toUser, out var to) || from.Balance < amount)
            {
                return false;
            }
            from.Balance -= amount;
            to.Balance += amount;
            SaveToFile();
            return true;
        }
    }
} 
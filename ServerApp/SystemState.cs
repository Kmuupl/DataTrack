using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices.Marshalling;

class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public decimal Balance { get; set; }
    public string Role { get; set; }

    public User(string username, string password, decimal balance, string role = "user")
    {
        Username = username;
        Password = password;
        Balance = balance;
        Role = role;
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
                { "admin", new User("admin", PasswordHelper.HashPassword("1234"), 1000m, "admin") },
                { "user", new User("user", PasswordHelper.HashPassword("pass"), 500m, "user") }
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
            return _users.TryGetValue(username, out var user) && user.Role != "deleted" && user.Password == PasswordHelper.HashPassword(password);
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

    public string GetRole(string username)
    {
        lock (_lock)
        {
            return _users.TryGetValue(username, out var u) ? u.Role : "user";
        }
    }

    public bool AddUser(string username, string password, decimal balance = 0m)
    {
        lock (_lock)
        {
            if (_users.ContainsKey(username)) return false;
            _users[username] = new User(username, PasswordHelper.HashPassword(password), balance);
            SaveToFile();
            return true;
        }
    }

    public bool DeleteUser(string username)
    {
        lock (_lock)
        {
            if (!_users.ContainsKey(username)) return false;
            _users[username].Role = "deleted";
            SaveToFile();
            return true;
        }
    }

    public bool EditUser(string username, string? newPassword, string? newName)
    {
        lock (_lock)
        {
            if (!_users.TryGetValue(username, out var u)) return false;
            if (newPassword != null) u.Password = PasswordHelper.HashPassword(newPassword);
            if (newName != null)
            {
                _users.Remove(username);
                u.Username = newName;
                _users[newName] = u;
            }
            SaveToFile();
            return true;
        }
    }

    public string ListUsers()
    {
        lock (_lock)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var u in _users.Values)
            {
                if (u.Role == "deleted")
                    sb.AppendLine($"[DELETED] {u.Username} | balance: {u.Balance}");
                else
                    sb.AppendLine($"{u.Username} | role: {u.Role} | balance: {u.Balance}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
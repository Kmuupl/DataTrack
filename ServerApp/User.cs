class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public decimal Balance { get; set; }
    public string Role { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public User(string username, string password, decimal balance = 0m, string role = "user", string firstName = "", string lastName = "")
    {
        Username = username;
        Password = password;
        Balance = balance;
        Role = role;
        FirstName = firstName;
        LastName = lastName;
    }
}
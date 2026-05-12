
class CommandHandler
{
    private SystemState _state;
    private Logger _logger;
    private string? _loggedInUser = null;

    public CommandHandler(SystemState state, Logger logger)
    {
        _state = state;
        _logger = logger;
    }

    public string Handle(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "No command entered.";
        }

        string[] parts = input.Split(' ');
        string command = parts[0].ToUpper();

        switch (command)
        {
            case "HELP":
                return "Commands: HELP, LOGIN <use> <pass>, STATUS, LOGOUT, LOGS, " +
                    "BALANCE, DEPOSIT <amount>, WITHDRAW <amount>, TRANSFER <user> <amount>";
            case "LOGIN":
                if (parts.Length < 3) return "Usage: LOGIN <user> <pass>";
                if (_loggedInUser != null) return "Already logged in.";
                if (_state.ValidateUser(parts[1], parts[2]))
                {
                    _loggedInUser = parts[1];
                    _logger.Log(_loggedInUser, "LOGIN");
                    return "Login successful.";
                }
                else
                {
                    return "Invalid username or password.";
                }
        }
        if (_loggedInUser == null)
        {
            return "Please login first.";
        }

        switch (command)
        {
            case "STATUS":
                return $"Logged in as: {_loggedInUser}";

            case "LOGOUT":
                _logger.Log(_loggedInUser, "LOGOUT");
                _loggedInUser = null;
                return "Logged out.";

            case "BALANCE":
                _logger.Log(_loggedInUser, "CHECK BALANCE");
                decimal bal = _state.GetBalance(_loggedInUser);
                return "Balance: " + bal;

            case "DEPOSIT":
                if (parts.Length < 2) return "Usage: DEPOSIT <amount>";
                if (!decimal.TryParse(parts[1], out decimal dep) || dep <= 0) return "Invalid amount.";
                _state.Deposit(_loggedInUser, dep);
                _logger.Log(_loggedInUser, "DEPOSIT " + dep);
                return "Deposited " + dep + ". New balance: " + _state.GetBalance(_loggedInUser);

            case "WITHDRAW":
                if (parts.Length < 2) return "Usage: WITHDRAW <amount>";
                if (!decimal.TryParse(parts[1], out decimal wth) || wth <= 0) return "Invalid amount.";
                if (!_state.Withdraw(_loggedInUser, wth)) return "Insufficient funds.";
                _logger.Log(_loggedInUser, "WITHDRAW " + wth);
                return "Withdrew " + wth + ". New balance: " + _state.GetBalance(_loggedInUser);

            case "TRANSFER":
                if (parts.Length < 3) return "Usage: TRANSFER <user> <amount>";
                if (!decimal.TryParse(parts[2], out decimal amt) || amt <= 0) return "Invalid amount.";
                if (!_state.UserExists(parts[1])) return "Recipient user does not exist.";
                if (parts[1] == _loggedInUser) return "Cannot transfer to self.";
                _logger.Log(_loggedInUser, $"TRANSFER {amt} TO {parts[1]}");
                return _state.Transfer(_loggedInUser, parts[1], amt)
                    ? $"Transferred {amt} to {parts[1]}. New balance: {_state.GetBalance(_loggedInUser)}"
                    : "Insufficient funds.";

            case "LOGS":
                return _logger.GetLogs(_loggedInUser);

            default:
                return "Unknown command. Type HELP for a list of commands.";
        }
    }
}
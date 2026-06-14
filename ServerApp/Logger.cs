using System;
using System.IO;

class Logger
{
    private readonly object _lock = new object();
    private const string LogFile = "logs.txt";

    public void Log(string username, string action)
    {
        string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {username}: {action}";
        Console.WriteLine(entry);

        lock (_lock)
        {
            File.AppendAllText(LogFile, entry + Environment.NewLine);
        }
    }

    public string GetLogs(string username)
    {
        lock (_lock)
        {
            if (!File.Exists(LogFile))
                return "No logs available.";

            var lines = File.ReadAllLines(LogFile);
            var userLogs = Array.FindAll(lines, line => line.Contains("] " + username + ":")); // фикс

            return userLogs.Length > 0 ? string.Join("\n", userLogs) : "No logs for user.";
        }
    }
}
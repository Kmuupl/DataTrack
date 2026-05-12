using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Specialized;

class TestClient
{
    private string _name;
    private string _host;
    private int _port;
    private List<(string command, int delayMs)> _commands;

    public TestClient(string name, string host, int port, List<(string, int)> commands)
    {
        _name = name;
        _host = host;
        _port = port;
        _commands = commands;
    }

    public void Run()
    {
        try
        {
            TcpClient client = new TcpClient(_host, _port);
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);

            Log("Connected");

            foreach (var (command, delayMs) in _commands)
            {
                Thread.Sleep(delayMs);

                writer.WriteLine(command);
                writer.Flush();

                string response = ReadResponse(reader);
                Log($"{command} >> {response}");
            }

            writer.WriteLine("quit");
            writer.Flush();
            ReadResponse(reader);

            client.Close();
            Log("Disconnected");
        }

        catch (Exception e)
        {
            Log("Error: " + e.Message);
        }
    }

    private string ReadResponse(StreamReader reader)
    {
        var sb = new StringBuilder();
        string? line;
        while ((line = reader.ReadLine()) != null && line != "END")
        {
            sb.AppendLine(line);
        }
        return sb.ToString().TrimEnd();
    }

    private void Log(string massage)
    {
        Console.WriteLine($"[{_name}] {massage}");
    }
}
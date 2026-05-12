using System;
using System.Net.Sockets;
using System.IO;
using System.Text;

public class Client
{
    public static void Main(string[] args)
    {
        TcpClient client;
        try
        {
            client = new TcpClient("localhost", 8080);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error connecting: " + e.Message);
            return;
        }

        Console.WriteLine("Connected to: " + client.Client.RemoteEndPoint);

        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream);

        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();

            if (line == null || line.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                writer.WriteLine("quit");
                writer.Flush();
                ReadResponse(reader);
                break;
            }

            writer.WriteLine(line);
            writer.Flush();
            ReadResponse(reader);
        }

        client.Close();
    }

    static void ReadResponse(StreamReader reader)
    {
        var sb = new StringBuilder();
        string? line;
        while ((line = reader.ReadLine()) != null && line != "END")
        {
            sb.AppendLine(line);
        }
        Console.WriteLine("Server: " + sb);
    }
}
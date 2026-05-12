using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

public class Server
{
    public static void Main(string[] args)
    {
        SystemState state = new SystemState();
        Logger logger = new Logger();
        TcpListener listener = new TcpListener(IPAddress.Any, 8080);
        listener.Start();
        Console.WriteLine("Server started on port 8080");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);
            TcpClient localClient = client;
            new Thread(() => HandleClient(localClient, state, logger)).Start();
        }
    }

    static void HandleClient(TcpClient client, SystemState state, Logger logger)
    {
        CommandHandler handler = new CommandHandler(state, logger);
        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream);

        try
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine("Received: " + line);

                if (line.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    writer.WriteLine("Bye!");
                    writer.WriteLine("END");
                    writer.Flush();
                    break;
                }

                string response = handler.Handle(line);
                foreach (string responseLine in response.Split('\n'))
                    writer.WriteLine(responseLine);
                writer.WriteLine("END");
                writer.Flush();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Client error: " + e.Message);
        }
        finally
        {
            client.Close();
            Console.WriteLine("Client disconnected");
        }
    }
}
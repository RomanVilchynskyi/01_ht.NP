using System.Net.Sockets;
using System.Net;

internal class Program
{
    static TcpListener listener;
    static List<StreamWriter> sw = new List<StreamWriter>();

    static void Broadcast(string message, StreamWriter exclude)
    {
        lock (sw)
        {
            foreach (var writer in sw)
            {
                if (writer != exclude)
                {
                    try
                    {
                        writer.WriteLine(message);
                        writer.Flush();
                    }
                    catch { }
                }
            }
        }
    }

    static void ClientAdd(TcpClient client)
    {
        var stream = client.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream);

        lock (sw) sw.Add(writer);

        try
        {
            while (true)
            {
                string message = reader.ReadLine()!;
                if (message == null) break;

                Console.WriteLine("recieved: " + message);
                Broadcast(message, writer);
            }
        }
        catch
        {
            Console.WriteLine("Client left");
        }
        finally
        {
            lock (sw) sw.Remove(writer);
            client.Close();
        }
    }
    static int MAX_CLIENTS = 5;
    static void Main()
    {
        listener = new TcpListener(IPAddress.Any, 8080);
        listener.Start();
        Console.WriteLine("Server started...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();

            lock (sw)
            {
                if (sw.Count >= MAX_CLIENTS)
                {
                    var stream = client.GetStream();
                    StreamWriter tmp = new StreamWriter(stream);
                    tmp.WriteLine("Try later");
                    tmp.Flush();
                    tmp.Close();
                    client.Close();
                    continue;
                }
            }

            Console.WriteLine("New client added");
            Thread thread = new Thread(() => ClientAdd(client));
            thread.Start();
        }
    }
}
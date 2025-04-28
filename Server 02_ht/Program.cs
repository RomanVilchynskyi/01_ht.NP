using System.Net.Sockets;
using System.Net;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        UdpClient server = new UdpClient(8080);
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

        string[] responses = new string[]
        {
            "Hello",
            "How are you",
            "Nice to meet you",
            "What's new",
            "Goodbye"
        };

        Console.WriteLine("Server started...");

        while (true)
        {
            byte[] data = server.Receive(ref clientEndPoint);
            string message = Encoding.UTF8.GetString(data);
            Console.WriteLine($"Recieved: {message} from {clientEndPoint}");

            Random rnd = new Random();
            string response = responses[rnd.Next(responses.Length)];
            byte[] responseData = Encoding.UTF8.GetBytes(response);
            server.Send(responseData, responseData.Length, clientEndPoint);
        }
    }
}
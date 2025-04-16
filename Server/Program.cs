using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Xml.Linq;

internal class Program
{
    static string address = "127.0.0.1";
    static int port = 4040;

    private static string GetStreetsByPostalCode(string postalCode)
    {
        try
        {
            string[] lines = File.ReadAllLines("streets.txt");
            var streets = new StringBuilder();
            bool firstStreet = true;

            foreach (var line in lines)
            {
                string[] parts = line.Split(' ');
                if (parts[0] == postalCode)
                {
                    if (!firstStreet)
                    {
                        streets.Append(", ");
                    }

                    for (int i = 1; i < parts.Length; i++)
                    {
                        streets.Append(parts[i] + " ");
                    }

                    firstStreet = false;
                }
            }

            if (streets.Length > 0)
            {
                return streets.ToString();
            }
            else
            {
                return "No streets found for this postal code";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    private static void Main(string[] args)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        try
        {
            listenSocket.Bind(endPoint);
            Console.WriteLine("Server started! Waiting for connection");

            while (true)
            {
                byte[] data = new byte[1024];
                listenSocket.ReceiveFrom(data, ref remoteEndPoint);

                string postalCode = Encoding.Unicode.GetString(data);
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} :: Received postal code: {postalCode} from {remoteEndPoint}");

                string streets = GetStreetsByPostalCode(postalCode);

                data = Encoding.Unicode.GetBytes(streets);
                listenSocket.SendTo(data, remoteEndPoint);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            listenSocket.Shutdown(SocketShutdown.Both);
            listenSocket.Close();
        }
    }

}

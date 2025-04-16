using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static string serverAddress = "127.0.0.1"; 
    private static int serverPort = 4040;  

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        string postalCode = txtZip.Text;  
        if (string.IsNullOrEmpty(postalCode))
        {
            MessageBox.Show("Please enter a postal code");
            return;
        }

        string result = await GetStreetsByPostalCode(postalCode);

        txtResult.Text = result;
    }

    private async Task<string> GetStreetsByPostalCode(string postalCode)
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        try
        {
            byte[] sendBytes = Encoding.UTF8.GetBytes(postalCode);
            socket.SendTo(sendBytes, serverEndPoint);

            byte[] receivedData = new byte[1024];
            int bytesReceived = socket.ReceiveFrom(receivedData, ref remoteEndPoint);

            string result = Encoding.UTF8.GetString(receivedData, 0, bytesReceived);

            return result;
        }
        catch (Exception ex)
        {

            return ex.Message;
        }
        finally
        {
            socket.Close(); 
        }
    }
}

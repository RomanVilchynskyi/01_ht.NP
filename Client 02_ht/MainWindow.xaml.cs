using System.Net.Sockets;
using System.Net;
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

namespace Client_02_ht;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private UdpClient client;
    private IPEndPoint serverEndPoint;
    public MainWindow()
    {
        InitializeComponent();
        client = new UdpClient();
        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveMessages()
    {
        while (true)
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref remoteEP);
                string message = Encoding.UTF8.GetString(data);
                Dispatcher.Invoke(() =>
                {
                    AddMessage($"Server: {message}");
                });
            }
            catch (Exception) { }
        }
    }

    private void AddMessage(string message)
    {
        string time = DateTime.Now.ToString("HH:mm");
        list.Items.Insert(0, $"{message} --- {time}");
    }
    private void SendBtn(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(iptb.Text) || string.IsNullOrWhiteSpace(porttb.Text))
            {
                MessageBox.Show("Please enter port and ip address");
                return;
            }

            string ip = iptb.Text;
            int port = int.Parse(porttb.Text);
            serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            string message = messagetb.Text;
            if (!string.IsNullOrEmpty(message))
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                client.Send(data, data.Length, serverEndPoint);

                AddMessage($"You: {message}");
                messagetb.Clear();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
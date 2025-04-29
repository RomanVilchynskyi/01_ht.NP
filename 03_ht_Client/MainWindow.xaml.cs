using System.IO;
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

namespace _03_ht_Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    TcpClient client;
    StreamReader reader;
    StreamWriter writer;
    Thread thread;

    public MainWindow()
    {
        InitializeComponent();
        ConnectToServer();
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 8080);
            var stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);

            thread = new Thread(ReceiveMessages);
            thread.IsBackground = true;
            thread.Start();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message);
        }
    }

    void ReceiveMessages()
    {
        try
        {
            while (true)
            {
                string message = reader.ReadLine()!;
                Dispatcher.Invoke(() => list.Items.Add(message));
            }
        }
        catch
        {
            Dispatcher.Invoke(() => list.Items.Add("Connection lost"));
        }
    }

    private void Send_Click(object sender, RoutedEventArgs e)
    {
        string name = namebox.Text;
        string text = messagebox.Text;
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(text)) return;

        string message = $"{name} [{DateTime.Now:HH:mm:ss}]: {text}";
        writer.WriteLine(message);
        writer.Flush();
        messagebox.Clear();
    }
}
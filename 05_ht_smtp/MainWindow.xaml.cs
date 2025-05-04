using Microsoft.Win32;
using System.Net.Mail;
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

namespace _05_ht_smtp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<string> attachments = new List<string>();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void SelectFile(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Multiselect = true;

        if (dialog.ShowDialog() == true)
        {
            foreach (string file in dialog.FileNames)
            {
                attachments.Add(file);
                listFiles.Items.Add(System.IO.Path.GetFileName(file));
            }
        }
    }

    private void SendMessage(object sender, RoutedEventArgs e)
    {
        string email = emailBox.Text;
        string password = passwordBox.Password;
        string to = toBox.Text;
        string subject = themeBox.Text;
        string body = new TextRange(messageBox.Document.ContentStart, messageBox.Document.ContentEnd).Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(to))
        {
            MessageBox.Show("Email, password and address are obilged");
            return;
        }

        try
        {
            MailMessage message = new MailMessage(email, to, subject, body)
            {
                IsBodyHtml = false,
                Priority = MailPriority.Normal
            };

            foreach (string file in attachments)
                message.Attachments.Add(new Attachment(file));

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587) 
            {
                Credentials = new NetworkCredential(email, password),
                EnableSsl = true
            };

            client.Send(message);
            MessageBox.Show("Message sent successfuly");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
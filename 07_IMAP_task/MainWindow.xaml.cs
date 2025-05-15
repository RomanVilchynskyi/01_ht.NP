using MimeKit;
using MailKit.Net.Smtp;
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
using MailKit.Net.Imap;
using MailKit;

namespace _07_IMAP_task;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<MimeMessage> emails = new List<MimeMessage>();
    private ImapClient imapClient;
    private string userEmail;
    private string userPassword;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void LoginBtn(object sender, RoutedEventArgs e)
    {
        userEmail = emailbox.Text;
        userPassword = passwordbox.Password;

        try
        {
            await LoginAndFetchMails(userEmail, userPassword);
            UpdateListBox(emails);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error when login downloasd lists:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task LoginAndFetchMails(string email, string password)
    {
        imapClient = new ImapClient();

        await imapClient.ConnectAsync("imap.gmail.com", 993, true);
        await imapClient.AuthenticateAsync(email, password);

        var inbox = imapClient.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite);

        emails.Clear();
        for (int i = 0; i < inbox.Count; i++)
        {
            var message = await inbox.GetMessageAsync(i);
            emails.Add(message);
        }
    }

    private async void ReplyBtn(object sender, RoutedEventArgs e)
    {
        if (list.SelectedIndex >= 0)
        {
            var original = emails[list.SelectedIndex];
            await SendMail(userEmail, original.From.Mailboxes.First().Address, "RE: " + original.Subject, messagebox.Text);
            MessageBox.Show("Sent");
        }
    }

    private async Task SendMail(string from, string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(from, from));
        message.To.Add(new MailboxAddress(to, to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(from, userPassword);
        await smtpClient.SendAsync(message);
        await smtpClient.DisconnectAsync(true);
    }

    private async void DeleteSelectedBtn(object sender, RoutedEventArgs e)
    {
        if (list.SelectedIndex >= 0)
        {
            await DeleteMail(list.SelectedIndex);
            UpdateListBox(emails);
        }
    }

    private async Task DeleteMail(int index)
    {
        var inbox = imapClient.Inbox;
        await inbox.AddFlagsAsync(index, MessageFlags.Deleted, true);
        await inbox.ExpungeAsync();
        emails.RemoveAt(index);
    }


    private void UpdateListBox(List<MimeMessage> messages)
    {
        list.Items.Clear();
        foreach (var message in messages)
        {
            list.Items.Add($"{message.Date}: {message.Subject}");
        }
    }

    private async void CreateFolderBtn(object sender, RoutedEventArgs e)
    {
        await CreateFolder("NewFolder");
    }

    private async Task CreateFolder(string folderName)
    {
        var personal = imapClient.GetFolder(imapClient.PersonalNamespaces[0]);
        await personal.CreateAsync(folderName, true);
    }

    private async void RenameFolderBtn(object sender, RoutedEventArgs e)
    {
        await RenameFolder("OldFolder", "RenamedFolder");
    }

    private async Task RenameFolder(string oldName, string newName)
    {
        var folder = await imapClient.GetFolderAsync(oldName);
        if (folder != null)
        {
            await folder.RenameAsync(null, newName);
        }
    }

    private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (list.SelectedIndex >= 0)
        {
            var message = emails[list.SelectedIndex];
            messagebox.Text = message.TextBody;
        }
    }
}
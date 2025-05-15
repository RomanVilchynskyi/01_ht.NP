using System.Windows;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Collections.Generic;
using MailKit;
using System.Linq;
using System.Windows.Controls;

namespace _07_ht_Imap
{
    public partial class MainWindow : Window
    {
        private ImapClient imapClient = new ImapClient();
        private SmtpClient smtpClient = new SmtpClient();
        private IList<MimeMessage> messages = new List<MimeMessage>();
        private string userEmail;
        private string userPassword;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoginBtn(object sender, RoutedEventArgs e)
        {
            userEmail = email.Text;
            userPassword = passwordbox.Password;

            try
            {
                await imapClient.ConnectAsync("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
                await imapClient.AuthenticateAsync(userEmail, userPassword);

                await smtpClient.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(userEmail, userPassword);

                combobox.Items.Clear();
                var folders = await imapClient.GetFoldersAsync(imapClient.PersonalNamespaces[0]);
                foreach (var folder in folders)
                    combobox.Items.Add(folder.FullName);

                MessageBox.Show("Успішно авторизовано.");
            }
            catch
            {
                MessageBox.Show("Помилка авторизації. Перевірте дані.");
            }
        }

        private async void FolderBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (combobox.SelectedItem == null) return;

            var folderName = combobox.SelectedItem.ToString();
            var folder = await imapClient.GetFolderAsync(folderName);
            await folder.OpenAsync(FolderAccess.ReadOnly);

            messages = new List<MimeMessage>();
            list.Items.Clear();

            for (int i = 0; i < folder.Count; i++)
            {
                var msg = await folder.GetMessageAsync(i);
                messages.Add(msg);
                list.Items.Add($"{msg.Date.LocalDateTime:g} — {msg.Subject}");
            }
        }

        private void MailListSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (list.SelectedIndex < 0) return;
            var msg = messages[list.SelectedIndex];

            mailbox.Text = $"Від: {msg.From}\nКому: {msg.To}\nДата: {msg.Date}\nТема: {msg.Subject}\n\n{msg.TextBody}";
        }
    }
}

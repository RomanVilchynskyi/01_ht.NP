using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

namespace _08_HTTP_ht;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

public class DownloadItem
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public WebClient Client { get; set; }
    public int Progress { get; set; }
    public string FileSize { get; set; }
    public string Status { get; set; }
}
public partial class MainWindow : Window
{
    private string selectedPath = "";
    private List<DownloadItem> downloads = new List<DownloadItem>();

    public MainWindow()
    {
        InitializeComponent();
        DownloadsPanel.ItemsSource = null;
    }

    private void Save_asBtn(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog();
        if (dialog.ShowDialog() == true)
        {
            selectedPath = dialog.FileName;
        }
    }

    private void DownloadBtn(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(UrlTextBox.Text) || string.IsNullOrEmpty(selectedPath))
        {
            MessageBox.Show("Enter url and choose file saving location");
            return;
        }

        var item = new DownloadItem
        {
            FileName = System.IO.Path.GetFileName(selectedPath),
            FilePath = selectedPath,
            Status = "Awaits...",
            Progress = 0
        };
        downloads.Add(item);

        RefreshDownloadsPanel();

        WebClient client = new WebClient();
        item.Client = client;

        client.DownloadProgressChanged += (s, ev) =>
        {
            item.Progress = ev.ProgressPercentage;
            item.FileSize = $"{ev.BytesReceived / 1024} KB / {ev.TotalBytesToReceive / 1024} KB";
            item.Status = $"Loading {ev.ProgressPercentage}%";
            RefreshDownloadsPanel();
        };

        client.DownloadFileCompleted += (s, ev) =>
        {
            if (ev.Error != null)
            {
                item.Status = ev.Error.Message;
            }
            else
            {
                item.Status = ev.Cancelled ? "Canceled" : "Finished";
            }
            RefreshDownloadsPanel();
        };

        try
        {
            item.Status = "Loading...";
            client.DownloadFileAsync(new Uri(UrlTextBox.Text), selectedPath);
        }
        catch (Exception ex)
        {
            item.Status = ex.Message;
            RefreshDownloadsPanel();
        }
    }

    private void OpenFileBtn(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var item = button?.Tag as DownloadItem;
        if (item != null && File.Exists(item.FilePath))
        {
            try
            {
                Process.Start(new ProcessStartInfo(item.FilePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    private void CancelBtn(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var item = button?.Tag as DownloadItem;
        if (item != null && item.Client != null && item.Client.IsBusy)
        {
            try
            {
                item.Client.CancelAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    private void RefreshDownloadsPanel()
    {
        DownloadsPanel.ItemsSource = null;
        DownloadsPanel.ItemsSource = downloads;
    }
}



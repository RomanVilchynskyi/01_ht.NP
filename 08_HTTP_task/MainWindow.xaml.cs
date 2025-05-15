using Microsoft.Win32;
using System.ComponentModel;
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

namespace _08_HTTP_task;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string savePath;
    private WebClient webClient;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        progressbar.Value = e.ProgressPercentage;
    }

    private void ChoosePathBtn(object sender, RoutedEventArgs e)
    {
        SaveFileDialog dlg = new SaveFileDialog();
        if (dlg.ShowDialog() == true)
        {
            savePath = dlg.FileName;
        }
    }
    private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Error != null)
        {
            list.Items.Add("Error while loding");
        }
        else if (e.Cancelled)
        {
            list.Items.Add("Loading cancalled");
        }
        else
        {
            list.Items.Add("Loading finished");
        }
        progressbar.Value = 0;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        string url = urlTextBox.Text;
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(savePath))
        {
            MessageBox.Show("Enter url and choose the location");
            return;
        }
    }
}
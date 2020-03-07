using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsyncDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            RunDownloadSync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time: { elapsedMs }";
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadParallelAsync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time: { elapsedMs }";
        }

        private List<string> PrepData()
        {
            List<string> output = new List<string>();

            resultsWindow.Text = "";

            output.Add("https://www.yahoo.com");
            output.Add("https://www.google.com");
            output.Add("https://www.microsoft.com");
            output.Add("https://www.cnn.com");
            output.Add("https://www.codeproject.com");
            output.Add("https://www.stackoverflow.com");

            return output;
        }

        private async Task RunDownloadAsync()
        {
            List<string> websites = PrepData();

            foreach (var site in websites)
            {
                WebSiteDataModel results = await Task.Run(() => DownloadWebSite(site));
                ReportWebSiteInfo(results);
            }
        }

        private async Task RunDownloadParallelAsync()
        {
            List<string> websites = PrepData();
            List<Task<WebSiteDataModel>> tasks = new List<Task<WebSiteDataModel>>();

            foreach (var site in websites)
            {
                tasks.Add(Task.Run(() => DownloadWebSite(site)));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var item in results)
            {
                ReportWebSiteInfo(item);
            }
        }

        private void RunDownloadSync()
        {
            List<string> websites = PrepData();

            foreach (var site in websites)
            {
                WebSiteDataModel results = DownloadWebSite(site);
                ReportWebSiteInfo(results);
            }
        }

        private WebSiteDataModel DownloadWebSite(string webSiteUrl)
        {
            WebSiteDataModel output = new WebSiteDataModel();
            WebClient client = new WebClient();

            output.WebSiteUrl = webSiteUrl;
            output.WebSiteData = client.DownloadString(webSiteUrl);

            return output;
        }

        private void ReportWebSiteInfo(WebSiteDataModel data)
        {
            resultsWindow.Text += $"{ data.WebSiteUrl } downloaded: { data.WebSiteData.Length } characters long. { Environment.NewLine }";
        }
    }
}

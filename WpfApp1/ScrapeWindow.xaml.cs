// ScrapeWindow.xaml.cs
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using HtmlAgilityPack;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class ScrapeWindow : Window
    {
        public ScrapeWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                var counts = new ConcurrentDictionary<string, int>();
                int limit = 1;
                var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };

                Parallel.For(1, limit + 1, options, i =>
                {
                    ScrapePage(i, counts);
                });

                var sortedCounts = counts.OrderByDescending(x => x.Value).ToList();

                // Use Dispatcher.Invoke to update UI elements on the main thread
                Dispatcher.Invoke(() =>
                {
                    dataGrid.ItemsSource = sortedCounts;
                });
            });
        }

        private void ScrapePage(int i, ConcurrentDictionary<string, int> counts)
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("--log-level=3"); // Suppresses all but fatal log messages

            // Path to your ChromeDriver
            string pathToChromeDriver = @"C:\chromedriver.exe";

            var driverService = ChromeDriverService.CreateDefaultService(pathToChromeDriver);
            driverService.HideCommandPromptWindow = true;

            using (var driver = new ChromeDriver(driverService, options))
            {
                driver.Navigate().GoToUrl($"https://www.ad.co.il/car?pageindex={i}");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElement(By.XPath("//h2[@class='card-title mb-0 mb-sm-1']")));

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(driver.PageSource);

                var carModels = htmlDoc.DocumentNode.SelectNodes("//h2[@class='card-title mb-0 mb-sm-1']");

                if (carModels != null)
                {
                    foreach (var model in carModels)
                    {
                        var modelText = model.InnerText.Trim();

                        counts.AddOrUpdate(modelText, 1, (_, count) => count + 1);
                    }
                }
            }
        }
    }
}

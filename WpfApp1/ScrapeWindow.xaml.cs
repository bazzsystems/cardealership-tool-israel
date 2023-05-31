using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace WpfApp1
{
    public partial class ScrapeWindow : Window
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<string, List<CarModel>>> carData;
        private CompareWindow compareWindow;

        public ScrapeWindow()
        {
            InitializeComponent();
            carData = new ConcurrentDictionary<string, ConcurrentDictionary<string, List<CarModel>>>();
            compareWindow = new CompareWindow(carData);
        }

        private async void StartScrapingButton_Click(object sender, RoutedEventArgs e)
        {
            await ScrapeCars(100);
        }

        private async void PullTrendingCars30Button_Click(object sender, RoutedEventArgs e)
        {
            await ScrapeCars(30);
        }

        private async void PullTrendingCars24Button_Click(object sender, RoutedEventArgs e)
        {
            await ScrapeCars(20);
        }

        private async Task ScrapeCars(int limit)
        {
            try
            {
                StartScrapingButton.IsEnabled = false;

                carData.Clear();

                var tasks = Enumerable.Range(1, limit)
                    .Select(i => Task.Run(() => ScrapePage(i)));

                await Task.WhenAll(tasks);

                UpdateDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                StartScrapingButton.IsEnabled = true;
            }
        }

        private void ScrapePage(int i)
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("--log-level=3");
            // remember to remove debug chrome driver
            var driverService = ChromeDriverService.CreateDefaultService(@"C:\chromedriver.exe");

            using (var driver = new ChromeDriver(driverService, options))
            {
                try
                {
                    driver.Navigate().GoToUrl($"https://www.ad.co.il/car?pageindex={i}");

                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(d => d.FindElement(By.XPath("//h2[@class='card-title mb-0 mb-sm-1']")));

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(driver.PageSource);

                    ExtractCarDataFromHtml(htmlDoc);
                }
                finally
                {
                    driver.Quit();
                }
            }
        }




        private void ExtractCarDataFromHtml(HtmlDocument htmlDoc)
        {
            var carNodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='card overflow-hidden']");
            if (carNodes != null)
            {
                foreach (var carNode in carNodes)
                {
                    var modelNode = carNode.SelectSingleNode(".//h2[@class='card-title mb-0 mb-sm-1']");
                    var priceNode = carNode.SelectSingleNode(".//div[@class='price ms-1']");
                    var mileageNode = carNode.SelectSingleNode(".//i[contains(@class, 'fa-tachometer-alt')]/following-sibling::span");

                    if (modelNode != null && priceNode != null && mileageNode != null)
                    {
                        var modelText = modelNode.InnerText.Trim();
                        var priceText = priceNode.InnerText.Trim().Replace(",", "").Replace("₪", "");
                        var mileageText = mileageNode.InnerText.Trim().Replace("K", "").Replace("ק\"מ", "");
                        var carModel = ExtractCarModelFromElement(modelText, priceText, mileageText);
                        if (carModel != null)
                        {
                            AddOrUpdateCarData(carModel);
                        }
                    }
                }
            }
        }

        private CarModel ExtractCarModelFromElement(string modelText, string priceText, string mileageText)
        {
            string brand = modelText.Split(" ")[0];
            string model = string.Join(" ", modelText.Split(" ").Skip(1));

            // Get actual values for year, price, and hands from HTML
            int year = 0; // assuming you still need to parse this from somewhere
            int price = int.TryParse(priceText, out int priceResult) ? priceResult : 0;
            int mileage = int.TryParse(mileageText, out int mileageResult) ? mileageResult : 0;
            int hands = 0; // assuming you still need to parse this from somewhere

            return new CarModel { Brand = brand, Model = model, Year = year, Price = price, Mileage = mileage, Hands = hands };
        }


        private CarModel ExtractCarModelFromElement(string modelText)
        {
            string brand = modelText.Split(" ")[0];
            string model = string.Join(" ", modelText.Split(" ").Skip(1));

            // Dummy values for year, price, and hands as they are not available in modelText
            int year = 0;
            int price = 0;
            int hands = 0;

            return new CarModel { Brand = brand, Model = model, Year = year, Price = price, Hands = hands };
        }

        private void AddOrUpdateCarData(CarModel carModel)
        {
            // Add brand to dictionary if it doesn't exist
            if (!carData.ContainsKey(carModel.Brand))
            {
                carData[carModel.Brand] = new ConcurrentDictionary<string, List<CarModel>>();
            }

            // Add model to brand if it doesn't exist
            if (!carData[carModel.Brand].ContainsKey(carModel.Model))
            {
                carData[carModel.Brand][carModel.Model] = new List<CarModel>();
            }

            // Add carModel to list of carModels for this brand and model
            carData[carModel.Brand][carModel.Model].Add(carModel);
        }


        private void UpdateDataGrid()
        {
            var flattenCarData = carData
                .SelectMany(brandEntry => brandEntry.Value
                    .Select(modelEntry => new
                    {
                        Brand = brandEntry.Key,
                        Model = modelEntry.Key,
                        Year = modelEntry.Value.Select(car => car.Year).FirstOrDefault(),
                        AveragePrice = modelEntry.Value.Select(car => car.Price).Average(),
                        AverageMileage = modelEntry.Value.Select(car => car.Mileage).Average(), // Add this line
                        AverageHands = modelEntry.Value.Select(car => car.Hands).Average(),
                        Count = modelEntry.Value.Count
                    }))
                .ToList();

            this.Dispatcher.Invoke(() =>
            {
                dataGrid.ItemsSource = flattenCarData;
            });
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            if (carData.Count == 0)
            {
                MessageBox.Show("Please scrape cars first before comparing.", "Scrape Cars", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (compareWindow == null || !compareWindow.IsLoaded)
            {
                compareWindow = new CompareWindow(carData);
            }

            compareWindow.Show();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchModel = SearchBox.Text.ToLower();
            var searchResult = carData
                .SelectMany(brandEntry => brandEntry.Value
                    .Where(modelEntry => modelEntry.Key.ToLower().Contains(searchModel))
                    .Select(modelEntry => new
                    {
                        Brand = brandEntry.Key,
                        Model = modelEntry.Key,
                        Year = modelEntry.Value.Select(car => car.Year).FirstOrDefault(),
                        AveragePrice = modelEntry.Value.Select(car => car.Price).Average(),
                        AverageMileage = modelEntry.Value.Select(car => car.Mileage).Average(), // Add this line
                        AverageHands = modelEntry.Value.Select(car => car.Hands).Average(),
                        Count = modelEntry.Value.Count
                    }))
                .ToList();

            dataGrid.ItemsSource = searchResult;
        }


        public class CarModel
        {
            public string Brand { get; set; }
            public string Model { get; set; }
            public int Year { get; set; }
            public int Price { get; set; }
            public int Mileage { get; set; }
            public int Hands { get; set; }
        }


    }
}

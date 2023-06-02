using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;

namespace WpfApp1
{
    public partial class LPCheckWindow : Window
    {
        public ObservableCollection<ScrapedDataItem> ScrapedData { get; set; }

        public LPCheckWindow()
        {
            InitializeComponent();
            ScrapedData = new ObservableCollection<ScrapedDataItem>();
            DataGrid.ItemsSource = ScrapedData;
        }

        private async void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            string licensePlateNumber = LicensePlateTextBox.Text;
            if (string.IsNullOrWhiteSpace(licensePlateNumber))
            {
                MessageBox.Show("Please enter a license plate number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await Task.Run(() =>
            {
                string url = $"https://www.check-car.co.il/report/{licensePlateNumber}/";

                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                var dataTables = doc.DocumentNode.SelectNodes("//div[@class='data_table']");

                if (dataTables != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ScrapedData.Clear();
                    });

                    foreach (var table in dataTables)
                    {
                        var tableCols = table.SelectNodes(".//div[@class='table_col']");

                        if (tableCols != null)
                        {
                            string category = tableCols[0].SelectSingleNode(".//span[@class='label']").InnerText;

                            foreach (var col in tableCols.Skip(1))
                            {
                                var label = col.SelectSingleNode(".//span[@class='label']").InnerText;
                                var value = col.SelectSingleNode(".//span[@class='value']").InnerText;

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    ScrapedData.Add(new ScrapedDataItem { Category = category, Label = label, Value = value });
                                });
                            }
                        }
                    }
                }
            });
        }
    }

    public class ScrapedDataItem
    {
        public string Category { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace WpfApp1
{
    public partial class CompareWindow : Window
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<string, List<ScrapeWindow.CarModel>>> carData;

        public CompareWindow(ConcurrentDictionary<string, ConcurrentDictionary<string, List<ScrapeWindow.CarModel>>> data)
        {
            InitializeComponent();
            carData = data;

            // Add the available car models to the ListBox
            foreach (var brand in carData.Keys)
            {
                foreach (var model in carData[brand].Keys)
                {
                    carListBox.Items.Add($"{brand} - {model}");
                }
            }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();

            carListBox.Items.Clear();

            foreach (var brand in carData.Keys)
            {
                foreach (var model in carData[brand].Keys)
                {
                    string carString = $"{brand} - {model}";

                    if (carString.ToLower().Contains(searchText))
                    {
                        carListBox.Items.Add(carString);
                    }
                }
            }
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedModels = carListBox.SelectedItems.Cast<string>().ToList();
            if (selectedModels.Count < 2)
            {
                MessageBox.Show("Please select at least two car models to compare.", "Compare Cars", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var seriesCollection = new SeriesCollection();
            foreach (var selectedModel in selectedModels)
            {
                var brandModel = selectedModel.Split(" - ");
                var brand = brandModel[0];
                var model = brandModel[1];

                if (carData.TryGetValue(brand, out var brandData) && brandData.TryGetValue(model, out var modelData))
                {
                    var prices = modelData.Select(car => new ObservableValue(car.Price)).ToList();
                    var lineSeries = new LineSeries { Title = selectedModel, Values = new ChartValues<ObservableValue>(prices) };
                    seriesCollection.Add(lineSeries);
                }
            }

            chart.Series = seriesCollection;
        }

        private void CarListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedModels = carListBox.SelectedItems.Cast<string>().ToList();
            CompareButton.IsEnabled = selectedModels.Count >= 2;
        }
    }
}

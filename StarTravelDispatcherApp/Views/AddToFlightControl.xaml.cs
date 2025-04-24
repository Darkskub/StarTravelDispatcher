using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using StarTravelDispatcherApp.Models;

namespace StarTravelDispatcherApp.Views
{
    public partial class AddToFlightControl : UserControl
    {
        private readonly HttpClient _httpClient;
        private readonly FlightDto _flight;

        public AddToFlightControl(HttpClient httpClient, FlightDto flight)
        {
            InitializeComponent();
            _httpClient = httpClient;
            _flight = flight;

            LoadPassengers();
        }

        private async void LoadPassengers()
        {
            try
            {
                var passengers = await _httpClient.GetFromJsonAsync<List<PassengerDto>>("/api/passengers");
                PassengersList.ItemsSource = passengers;
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки пассажиров.");
            }
        }

        private async void AddToFlight_Click(object sender, RoutedEventArgs e)
        {
            var selected = PassengersList.SelectedItems;
            if (selected.Count == 0)
            {
                MessageBox.Show("Выберите пассажиров.");
                return;
            }

            var ids = new List<int>();
            foreach (PassengerDto p in selected)
                ids.Add(p.Id);

            var response = await _httpClient.PostAsJsonAsync($"/api/flights/{_flight.Id}/passengers", ids);
            MessageBox.Show(response.IsSuccessStatusCode ? "Добавлено." : "Ошибка добавления");
        }

        private async void DeletePassenger_Click(object sender, RoutedEventArgs e)
        {
            if (PassengersList.SelectedItem is not PassengerDto p) return;
            var result = MessageBox.Show($"Удалить {p.FullName}?", "Удаление", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;

            var resp = await _httpClient.DeleteAsync($"/api/passengers/{p.Id}");
            if (resp.IsSuccessStatusCode) LoadPassengers();
            else MessageBox.Show("Ошибка удаления.");
        }

        private void AddPassenger_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddPassengerDialog();
            if (dialog.ShowDialog() == true)
            {
                _ = _httpClient.PostAsJsonAsync("/api/passengers", dialog.Result)
                    .ContinueWith(_ => LoadPassengers());
            }
        }
    }
}

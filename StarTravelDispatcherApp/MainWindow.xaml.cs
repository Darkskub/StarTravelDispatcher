using StarTravelDispatcherApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using StarTravelDispatcherApp.Models;

namespace StarTravelDispatcherApp
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;

        public MainWindow(HttpClient httpClient)
        {
            InitializeComponent();
            _httpClient = httpClient;

            LoadFlights(); // Загрузка рейсов при запуске
        }

        private void AddFlight_Click(object sender, RoutedEventArgs e)
        {
            RightPanel.Content = new AddFlightControl(_httpClient);
        }

        private void StarMap_Click(object sender, RoutedEventArgs e)
        {
            RightPanel.Content = new StarMapControl(_httpClient);
        }

        private void AssignToFlight_Click(object sender, RoutedEventArgs e)
        {
            if (FlightsList.SelectedItem is FlightDto flight)
                RightPanel.Content = new AddToFlightControl(_httpClient, flight);
        }

        private async void DeleteFlight_Click(object sender, RoutedEventArgs e)
        {
            if (FlightsList.SelectedItem is not FlightDto flight)
            {
                MessageBox.Show("Выберите полёт для удаления.");
                return;
            }

            var result = MessageBox.Show($"Удалить рейс #{flight.Id}?", "Подтверждение", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;

            var response = await _httpClient.DeleteAsync($"/api/flights/{flight.Id}");
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Рейс удалён.");
                await LoadFlights();
            }
            else
            {
                MessageBox.Show("Ошибка при удалении.");
            }
        }

        private async Task LoadFlights()
        {
            try
            {
                var flights = await _httpClient.GetFromJsonAsync<List<FlightViewDto>>("/api/flights");
                FlightsList.ItemsSource = flights;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки рейсов: {ex.Message}");
            }
        }

        private async void RemoveFromFlight_Click(object sender, RoutedEventArgs e)
        {
            if (FlightsList.SelectedItem is not FlightDto flight)
            {
                MessageBox.Show("Выберите рейс.");
                return;
            }

            var selected = PassengersList.SelectedItems.Cast<PassengerDto>().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Выберите пассажиров.");
                return;
            }

            var confirm = MessageBox.Show($"Удалить {selected.Count} пассажиров с рейса?", "Удаление", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;

            var ids = selected.Select(p => p.Id).ToList();

            var response = await _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"/api/flights/{flight.Id}/passengers", UriKind.Relative),
                Content = JsonContent.Create(ids)
            });

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Удалено.");
                await LoadPassengersForFlight(flight.Id);
            }
            else
            {
                MessageBox.Show("Ошибка при удалении.");
            }
        }

        private async Task LoadPassengersForFlight(int flightId)
        {
            try
            {
                var passengers = await _httpClient.GetFromJsonAsync<List<PassengerDto>>($"/api/flights/{flightId}/passengers");
                PassengersList.ItemsSource = passengers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пассажиров: {ex.Message}");
            }
        }

        private async void FlightsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FlightsList.SelectedItem is FlightDto flight)
            {
                await LoadPassengersForFlight(flight.Id);
            }
        }
    }
}

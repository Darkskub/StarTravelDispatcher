// MainWindow.xaml.cs
using StarTravelDispatcherApp.Models;
using StarTravelDispatcherApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace StarTravelDispatcherApp
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private List<PassengerDto> _allPassengers = new();
        private List<PassengerDto> _flightPassengers = new();
        private FlightPassengersControl? _flightPassengersControl;
        private int? _selectedFlightId = null;

        public MainWindow(HttpClient httpClient)
        {
            InitializeComponent();
            _httpClient = httpClient;
            LoadFlights();
            LoadAllPassengers();
        }

        private async Task LoadAllPassengers()
        {
            try
            {
                _allPassengers = await _httpClient.GetFromJsonAsync<List<PassengerDto>>("/api/passengers") ?? new();
                UpdatePassengerListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пассажиров: {ex.Message}");
            }
        }

        private void UpdatePassengerListView()
        {
            var query = PassengerSearchBox.Text?.Trim().ToLower() ?? "";
            var baseList = _allPassengers.OrderByDescending(p => p.IsAssigned).ThenBy(p => p.FullName).ToList();
            var filtered = string.IsNullOrEmpty(query)
                ? baseList
                : baseList.Where(p => p.FullName.ToLower().Contains(query)).ToList();

            PassengersList.ItemsSource = filtered;
        }

        private async Task LoadPassengersForFlight(int flightId)
        {
            try
            {
                _selectedFlightId = flightId;

                var assigned = await _httpClient.GetFromJsonAsync<List<PassengerDto>>($"/api/flights/{flightId}/passengers");
                var assignedIds = assigned.Select(p => p.Id).ToHashSet();

                foreach (var p in _allPassengers)
                    p.IsAssigned = assignedIds.Contains(p.Id);

                _flightPassengers = assigned;
                UpdatePassengerListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пассажиров рейса: {ex.Message}");
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

        private void PassengerSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePassengerListView();
        }

        private async void FlightsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FlightsList.SelectedItem is not FlightViewDto flight)
                return;

            _selectedFlightId = flight.Id;

            await LoadPassengersForFlight(flight.Id);
            SortPassengers();

            if (_flightPassengersControl == null || !(RightPanel.Content is FlightPassengersControl))
            {
                _flightPassengersControl = new FlightPassengersControl(_httpClient, flight.Id);
                RightPanel.Content = _flightPassengersControl;
            }
            else
            {
                await _flightPassengersControl.LoadForFlightAsync(flight.Id);
            }
        }

        private void SortPassengers()
        {
            if (_selectedFlightId == null)
                return;

            var sorted = _allPassengers
                .OrderByDescending(p => p.IsAssigned) // записанные на рейс — вверх
                .ThenBy(p => p.FullName)
                .ToList();

            PassengersList.ItemsSource = sorted;
        }


        private async void AddPassenger_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddPassengerDialog();
            if (dialog.ShowDialog() == true)
            {
                var response = await _httpClient.PostAsJsonAsync("/api/passengers", dialog.Result);
                if (response.IsSuccessStatusCode)
                {
                    await LoadAllPassengers();
                    if (_selectedFlightId.HasValue)
                        await LoadPassengersForFlight(_selectedFlightId.Value);
                }
            }
        }

        private async void DeletePassenger_Click(object sender, RoutedEventArgs e)
        {
            if (PassengersList.SelectedItem is not PassengerDto p) return;

            var confirm = MessageBox.Show($"Удалить {p.FullName}?", "Удаление", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;

            var response = await _httpClient.DeleteAsync($"/api/passengers/{p.Id}");
            if (response.IsSuccessStatusCode)
            {
                await LoadAllPassengers();
                if (_selectedFlightId.HasValue)
                    await LoadPassengersForFlight(_selectedFlightId.Value);
            }
        }

        private async void AssignToFlight_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFlightId == null)
            {
                MessageBox.Show("Выберите рейс.");
                return;
            }

            var selected = PassengersList.SelectedItems.Cast<PassengerDto>().ToList();
            if (!selected.Any())
            {
                MessageBox.Show("Выберите пассажиров.");
                return;
            }

            var ids = selected.Select(p => p.Id).ToList();
            var response = await _httpClient.PostAsJsonAsync($"/api/flights/{_selectedFlightId}/passengers", ids);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Пассажиры добавлены!");
                await Task.Delay(200);
                await LoadAllPassengers();
                await LoadPassengersForFlight(_selectedFlightId.Value);

                if (_flightPassengersControl != null)
                    await _flightPassengersControl.LoadForFlightAsync(_selectedFlightId.Value);
            }
            else
            {
                MessageBox.Show("Ошибка добавления.");
            }
        }

        private async void RemoveFromFlight_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFlightId == null)
            {
                MessageBox.Show("Выберите рейс.");
                return;
            }

            var selectedFromList = PassengersList.SelectedItems.Cast<PassengerDto>().Select(p => p.Id).ToList();
            var selectedFromControl = _flightPassengersControl?.GetSelectedPassengerIds() ?? new List<int>();

            var ids = selectedFromList.Union(selectedFromControl).Distinct().ToList();
            if (!ids.Any())
            {
                MessageBox.Show("Выберите пассажиров.");
                return;
            }

            var confirm = MessageBox.Show($"Удалить {ids.Count} пассажиров с рейса?", "Удаление", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/flights/{_selectedFlightId}/passengers")
            {
                Content = JsonContent.Create(ids)
            };

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Удалено.");
                await LoadAllPassengers();
                await LoadPassengersForFlight(_selectedFlightId.Value);

                if (_flightPassengersControl != null)
                    await _flightPassengersControl.LoadForFlightAsync(_selectedFlightId.Value);
            }
            else
            {
                MessageBox.Show("Ошибка при удалении.");
            }
        }

        private void AddFlight_Click(object sender, RoutedEventArgs e)
        {
            var control = new AddFlightControl(_httpClient);
            control.FlightAdded += async (_, _) => await LoadFlights();
            RightPanel.Content = control;
        }

        private void StarMap_Click(object sender, RoutedEventArgs e)
        {
            RightPanel.Content = new StarMapControl(_httpClient);
        }

        private async void DeleteFlight_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFlightId == null || FlightsList.SelectedItem is not FlightViewDto flight)
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
        
        private async void RefreshFlights_Click(object sender, RoutedEventArgs e)
        {
            await LoadFlights();
        }

        private void ShowAllPassengers_Click(object sender, RoutedEventArgs e)
        {
            var window = new AllPassengersWindow(_httpClient);
            window.Show();
        }

    }
}

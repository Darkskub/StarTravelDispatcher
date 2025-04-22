using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using StarTravelDispatcherApp.Models;

namespace StarTravelDispatcherApp.Views
{
    public partial class AddFlightControl : UserControl
    {
        private readonly HttpClient _httpClient;

        public AddFlightControl(HttpClient httpClient)
        {
            InitializeComponent();
            _httpClient = httpClient;

            LoadShipsAndStars();
        }

        private async void LoadShipsAndStars()
        {
            try
            {
                var ships = await _httpClient.GetFromJsonAsync<List<ShipDto>>("/api/ships");
                var stars = await _httpClient.GetFromJsonAsync<List<StarSystemDto>>("/api/starsystems");

                ShipComboBox.ItemsSource = ships;
                StarSystemsListBox.ItemsSource = stars;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private async void AddFlight_Click(object sender, RoutedEventArgs e)
        {
            if (ShipComboBox.SelectedItem is not ShipDto selectedShip)
            {
                MessageBox.Show("Выбери корабль.");
                return;
            }

            var selectedStars = new List<int>();
            foreach (StarSystemDto star in StarSystemsListBox.SelectedItems)
                selectedStars.Add(star.Id);

            if (selectedStars.Count < 2)
            {
                MessageBox.Show("Нужно минимум 2 звёздные системы.");
                return;
            }

            var dto = new FlightDto
            {
                ShipId = selectedShip.Id,
                Date = FlightDatePicker.SelectedDate ?? DateTime.Today,
                StarSystemIds = selectedStars
            };

            var response = await _httpClient.PostAsJsonAsync("/api/flights", dto);
            if (response.IsSuccessStatusCode)
                MessageBox.Show("Рейс добавлен!");
            else
                MessageBox.Show("Ошибка при добавлении рейса.");
        }
    }
}

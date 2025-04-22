using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using StarTravelDispatcherApp.Models;

namespace StarTravelDispatcherApp.Views
{
    public partial class StarMapControl : UserControl
    {
        private readonly HttpClient _httpClient;

        public StarMapControl(HttpClient httpClient)
        {
            InitializeComponent();
            _httpClient = httpClient;
            LoadStars();
        }

        private async void LoadStars()
        {
            try
            {
                var stars = await _httpClient.GetFromJsonAsync<List<StarSystemDto>>("/api/starsystems");
                StarListBox.ItemsSource = stars;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private async void AddSystem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddStarDialog();
            if (dialog.ShowDialog() == true)
            {
                var newStar = dialog.Result;
                var response = await _httpClient.PostAsJsonAsync("/api/starsystems", newStar);
                if (response.IsSuccessStatusCode)
                    LoadStars();
                else
                    MessageBox.Show("Ошибка добавления.");
            }
        }

        private async void DeleteSystem_Click(object sender, RoutedEventArgs e)
        {
            if (StarListBox.SelectedItem is not StarSystemDto star) return;

            var response = await _httpClient.DeleteAsync($"/api/starsystems/{star.Id}");
            if (response.IsSuccessStatusCode)
                LoadStars();
            else
                MessageBox.Show("Ошибка удаления.");
        }

        private void StarListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (StarListBox.SelectedItem is StarSystemDto star)
            {
                MessageBox.Show($"Координаты {star.Name}: X={star.X}, Y={star.Y}");
            }
        }
    }
}

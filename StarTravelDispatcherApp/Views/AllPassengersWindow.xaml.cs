using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using StarTravelDispatcherApp.Models;
using System.Net.Http.Json;

namespace StarTravelDispatcherApp.Views
{
    /// <summary>
    /// Interaction logic for AllPassengersWindow.xaml
    /// </summary>
    public partial class AllPassengersWindow : Window
    {
        private readonly HttpClient _httpClient;

        public AllPassengersWindow(HttpClient client)
        {
            InitializeComponent();
            _httpClient = client;
            LoadPassengers();
        }

        private async void LoadPassengers()
        {
            try
            {
                var passengers = await _httpClient.GetFromJsonAsync<List<PassengerDto>>("/api/passengers/sql");
                AllPassengersList.ItemsSource = passengers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }

}

using StarTravelDispatcherApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StarTravelDispatcherApp.Views
{
    public partial class FlightPassengersControl : UserControl
    {
        private readonly HttpClient _httpClient;
        private int _flightId;

        public FlightPassengersControl(HttpClient httpClient, int flightId)
        {
            InitializeComponent();
            _httpClient = httpClient;
            _flightId = flightId;
            _ = LoadPassengers();
        }

        public async Task LoadForFlightAsync(int flightId)
        {
            _flightId = flightId;
            await LoadPassengers();
        }

        private async Task LoadPassengers()
        {
            try
            {
                var passengers = await _httpClient.GetFromJsonAsync<List<PassengerDto>>(
                    $"/api/flights/{_flightId}/passengers");

                FlightPassengersList.ItemsSource = passengers;
            }
            catch
            {
                // Логгирование можно добавить
            }
        }

        public List<int> GetSelectedPassengerIds()
        {
            var selected = new List<int>();
            foreach (var item in FlightPassengersList.SelectedItems)
            {
                if (item is PassengerDto p)
                    selected.Add(p.Id);
            }
            return selected;
        }
    }
}

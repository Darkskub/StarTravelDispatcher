using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace StarTravelDispatcherApp
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("http://localhost:5000")
        };

        public LoginWindow()
        {
            InitializeComponent();
        }

        private static string? _token;

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var loginDto = new
            {
                Username = UsernameBox.Text.Trim(),
                Password = PasswordBox.Password
            };

            var response = await _httpClient.PostAsJsonAsync("/api/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var token = (await response.Content.ReadAsStringAsync()).Trim('"');

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var main = new MainWindow(_httpClient);
                main.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }
    }
}

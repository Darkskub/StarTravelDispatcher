using System.Windows;
using StarTravelDispatcherApp.Models;

namespace StarTravelDispatcherApp.Views
{
    public partial class AddPassengerDialog : Window
    {
        public PassengerDto Result { get; private set; } = null!;

        public AddPassengerDialog()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var name = FullNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите ФИО.");
                return;
            }

            Result = new PassengerDto { FullName = name };
            DialogResult = true;
            Close();
        }
    }
}

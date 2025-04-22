using System.Windows;
using StarTravelDispatcherApp.Models; // обязательно подключить DTO

namespace StarTravelDispatcherApp.Views
{
    public partial class AddStarDialog : Window
    {
        public StarSystemDto Result { get; private set; } = null!;

        public AddStarDialog()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(XBox.Text, out int x) || !int.TryParse(YBox.Text, out int y))
            {
                MessageBox.Show("Координаты должны быть целыми числами.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Название не может быть пустым.");
                return;
            }

            Result = new StarSystemDto
            {
                Name = NameBox.Text.Trim(),
                X = x,
                Y = y
            };

            DialogResult = true;
            Close();
        }
    }
}

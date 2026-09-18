using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _4.Ariketa.IgorGarcia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string erabiltzailea = "Informatika";
        private const string pasahitza = "1234";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Garbitu_Click(object sender, RoutedEventArgs e)
        {
            txtErabiltzailea.Text = string.Empty;
            txtPassword.Password = string.Empty;
        }

        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Onartu_Click(object sender, RoutedEventArgs e)
        {
            if (txtErabiltzailea.Text==erabiltzailea && txtPassword.Password==pasahitza)
            {
                lblEmaitza.Content = $"Ongi etorri sistemara,{txtErabiltzailea.Text}";
            }
            else
            {
                lblEmaitza.Content = "Identifikatu gabeko erabiltzailea";
            }
        }

    }
}
using AriketaMultzoa3;
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

namespace _11.Ariketa.IgorGarcia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Onartu_Click(object sender, RoutedEventArgs e)
        {
            Datuak.Izena = txtIzena.Text;
            Datuak.Abizena1 = txtAbizena1.Text;
            Datuak.Abizena2 = txtAbizena2.Text;
            Datuak.NAN = txtNan.Text;
        }
        private void Kargatu_Click(object sender, RoutedEventArgs e)
        {
            Bistaratu bistaratu = new Bistaratu();
            bistaratu.ShowDialog();
        }
        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
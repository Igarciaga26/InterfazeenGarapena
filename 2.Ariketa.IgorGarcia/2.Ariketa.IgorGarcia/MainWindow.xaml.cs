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

namespace _2.Ariketa.IgorGarcia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> esaldiak = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            HasieratuEgoera();
        }

        private void Esaldia1_Click(object sender, RoutedEventArgs e)
        {
            Esaldia_Gorde(Esaldia1, Esaldia2);
        }
        private void Esaldia2_Click(object sender, RoutedEventArgs e)
        {
            Esaldia_Gorde(Esaldia2, Esaldia3);
        }
        private void Esaldia3_Click(object sender, RoutedEventArgs e)
        {
            Esaldia_Gorde(Esaldia3, Esaldia4);
        }
        private void Esaldia4_Click(object sender, RoutedEventArgs e)
        {
            Esaldia_Gorde(Esaldia4, Esaldia5);
        }
        private void Esaldia5_Click(object sender, RoutedEventArgs e)
        {
            Esaldia_Gorde(Esaldia5, Batu);
        }
        private void Batu_Click(object sender, RoutedEventArgs e)
        {
            TestuKutxa.Text = string.Join(" ", esaldiak);
            Batu.IsEnabled = false;
        }

        private void Esaldia_Gorde(UIElement unekoBotoia, UIElement hurrengoBotoia)
        {
            esaldiak.Add(TestuKutxa.Text);
            TestuKutxa.Clear();
            unekoBotoia.IsEnabled = false;
            hurrengoBotoia.IsEnabled = true;
            TestuKutxa.Focus();
        }

        private void HasieratuEgoera()
        {
            esaldiak.Clear();
            TestuKutxa.Clear();

            Esaldia1.IsEnabled = true;
            Esaldia2.IsEnabled = false;
            Esaldia3.IsEnabled = false;
            Esaldia4.IsEnabled = false;
            Esaldia5.IsEnabled = false;
            Batu.IsEnabled = false;

            TestuKutxa.Focus();
        }
        private void Garbitu_Click(object sender, RoutedEventArgs e)
        {
            HasieratuEgoera();
        }

        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
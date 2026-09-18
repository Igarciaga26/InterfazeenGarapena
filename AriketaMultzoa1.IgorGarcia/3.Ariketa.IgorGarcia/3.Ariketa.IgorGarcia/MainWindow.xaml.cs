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

namespace _3.Ariketa.IgorGarcia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<double> zenbakiak = new List<double>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Hurrengoa_Click(object sender, RoutedEventArgs e)
        {
            if(double.TryParse(TxtZenbakia.Text, out double zenbakia))
            {
                zenbakiak.Add(zenbakia);
                TxtZenbakia.Clear();
                if(zenbakiak.Count<4)
                {
                    LblZenbakia.Content = $"{zenbakiak.Count + 1}. Zenbakia";
                }
                else
                {
                    double zenb = zenbakiak[0];
                    double zenb2 = zenbakiak[1];
                    double zenb3 = zenbakiak[2];
                    double zenb4 = zenbakiak[3];

                    double emaitza = (zenb + (zenb * zenb2) + (zenb2 * zenb3) + (zenb3 * zenb4)) / 4;

                    EmaitzaLeihoa bigarrenLeihoa = new EmaitzaLeihoa(emaitza);
                    this.Hide();
                    bigarrenLeihoa.ShowDialog();

                    if (bigarrenLeihoa.GarbituNahiDa)
                    {
                        HasieratuEgoera();
                        this.Show();
                    }
                }
            }
        }
        private void HasieratuEgoera()
        {
            zenbakiak.Clear();
            TxtZenbakia.Clear();
            LblZenbakia.Content = "1. Zenbakia";
            TxtZenbakia.Focus();
        }
        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
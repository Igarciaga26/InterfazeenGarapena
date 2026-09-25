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

namespace _8.Ariketa.IgorGarcia
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

        private void Garbitu_Click(object sender, RoutedEventArgs e)
        {
            txtOrain.Clear();
            txtGaur.Clear();
            txtGaurkoOrdua.Clear();
            txtDatenBatura.Clear();
            txtDatenAldea.Clear();
        }
        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Exekutatu_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                txtOrain.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                txtGaur.Text = DateTime.Today.ToString("dd/MM/yyyy");
                txtGaurkoOrdua.Text = DateTime.Now.ToString("HH:mm:ss");

                var data1 = new DatenAldea("Daten batura", "Sartu hasierako data honela:\ndd/mm/uuuu");
                if (data1.ShowDialog() != true) throw new FormatException();

                var hilabeteak = new Hilabeteak();
                if (hilabeteak.ShowDialog() != true) throw new FormatException();

                DateTime hasierakoData = data1.Data.Value;
                int hilabete1 = hilabeteak.Kopurua.Value;
                DateTime databerria = hasierakoData.AddMonths(hilabete1);
                txtDatenBatura.Text = $"Hasierako data:{hasierakoData}, Gehitutako hilabeteak: {hilabete1}, Data berria: {databerria:dd/MM/yyyy}";

                var data2 = new DatenAldea("Daten aldea", "Sartu hasierako data honela:\ndd/mm/uuuu");
                if (data2.ShowDialog() != true) throw new FormatException();
                var data3 = new DatenAldea("Daten aldea", "Sartu amaierako data honela:\ndd/mm/uuuu");
                if (data3.ShowDialog() != true) throw new FormatException();

                DateTime hasieraData = data2.Data.Value;
                DateTime bukaeraData = data3.Data.Value;
                int egunKop = (bukaeraData - hasieraData).Days;
                txtDatenAldea.Text = $"{hasieraData:dd/MM/yyyy}etik {bukaeraData:dd/MM/yyyy}ra {egunKop} egun daude";
            }
            catch
            {
                MessageBox.Show("Sartu datuak ondo edo sakatu Irten", "Datu okerrak", MessageBoxButton.OK, MessageBoxImage.Error);
                Garbitu_Click(sender, e);

            }
        }
    }
}
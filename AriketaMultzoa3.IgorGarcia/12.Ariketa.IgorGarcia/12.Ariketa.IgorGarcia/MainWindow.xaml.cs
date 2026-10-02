using System.Globalization;
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

namespace _12.Ariketa.IgorGarcia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const double GOSARIA = 3;
        const double BAZKARIA = 9;
        const double AFARIA = 15.5;
        const double KM = 0.25;
        const double BIDAIA_ORDUA = 18;
        const double LAN_ORDUA = 42;
        public MainWindow()
        {
            InitializeComponent();
        }

        private double Irakurri(TextBox txt)
        {
            double balioa;
            if (double.TryParse(txt.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out balioa))
                return balioa;
            return 0;
        }

        private void Kalkulatu(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            double dietak = 0;
            if (chkGosaria.IsChecked == true) dietak += GOSARIA;
            if (chkBazkaria.IsChecked == true) dietak += BAZKARIA;
            if (chkAfaria.IsChecked == true) dietak += AFARIA;
            double bidaiak = Irakurri(txtKM) * KM + Irakurri(txtOrduak) * BIDAIA_ORDUA;
            double lana = Irakurri(txtLanOrduak) * LAN_ORDUA;

            txt1.Text = dietak.ToString("N2") + " €";
            txt2.Text = bidaiak.ToString("N2") + " €";
            txt3.Text = lana.ToString("N2") + " €";
            txtGuztira.Text = (dietak + bidaiak + lana).ToString("N2") + " €";
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && e.OriginalSource is UIElement elemento && !(e.OriginalSource is Button))
            {
                elemento.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
            }
        }

        private void Garbitu_Click(object sender, RoutedEventArgs e)
        {
            chkGosaria.IsChecked = false;
            chkBazkaria.IsChecked = false;
            chkAfaria.IsChecked = false;
            txtKM.Clear();
            txtOrduak.Clear();
            txtLanOrduak.Clear();
            txt1.Clear();
            txt2.Clear();
            txt3.Clear();
            txtGuztira.Clear();

        }
        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
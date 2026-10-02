using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace _10.Ariketa.IgorGarcia
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, string> irudiakCombo = new Dictionary<string, string>
        {
            { "Irudia1", "Images/irudia1.jpg" },
            { "Irudia2", "Images/irudia2.jpg" },
            { "Irudia3", "Images/irudia3.jpg" }
        };

        public MainWindow()
        {
            InitializeComponent();

            comboIrudi.Items.Add("Irudia1");
            comboIrudi.Items.Add("Irudia2");
            comboIrudi.Items.Add("Irudia3");
            comboIrudi.SelectedIndex = 0;
        }

        private BitmapImage? KargatuIrudia(string pathRelativa)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pathRelativa);

            if (!File.Exists(path))
            {
                MessageBox.Show("Argazkia ez da aurkitzen:\n" + path);
                return null;
            }

            return new BitmapImage(new Uri(path, UriKind.Absolute));
        }

        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void comboIrudi_Aukera(object sender, RoutedEventArgs e)
        {
            if (comboIrudi.SelectedItem == null) return;
            string aukeratuta = comboIrudi.SelectedItem.ToString()!;

            if (irudiakCombo.ContainsKey(aukeratuta))
            {
                imgCombo.Source = KargatuIrudia(irudiakCombo[aukeratuta]);
            }
        }

        private void checkBox_Aukera(object sender, RoutedEventArgs e)
        {
            imgIrudia4.Visibility = chkIrudia4.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            imgIrudia5.Visibility = chkIrudia5.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            imgIrudia6.Visibility = chkIrudia6.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

            if (chkIrudia4.IsChecked == true)
            {
                imgIrudia4.Source = CargarImagen("Images/irudia4.jpg");
            }
            if (chkIrudia5.IsChecked == true)
            {
                imgIrudia5.Source = CargarImagen("Images/irudia5.jpg");
            }
            if (chkIrudia6.IsChecked == true)
            {
                imgIrudia6.Source = CargarImagen("Images/irudia6.jpg");
            }
        }
    }
}
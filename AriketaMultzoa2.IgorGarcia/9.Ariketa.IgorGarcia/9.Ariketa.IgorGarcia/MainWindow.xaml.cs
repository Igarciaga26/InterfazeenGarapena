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

namespace _9.Ariketa.IgorGarcia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LagunakList.Items.Add("Ana");
            LagunakList.Items.Add("Angel");
            LagunakList.Items.Add("Luis");
            LagunakList.Items.Add("Maria");
            LagunakList.Items.Add("Pedro");
            LagunakList.Items.Add("Sandra");

        }

        private void Gehitu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGehitu.Text))
            {
                MessageBox.Show("Sartu datuak gehitu ahal izateko", "Errorea gehitzean", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LagunakList.Items.Add(txtGehitu.Text);
            txtGehitu.Clear();
        }
        private void Ezabatu_Click(object sender, RoutedEventArgs e)
        {
            if (LagunakList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Hautatu ezabatu behar diren datuak", "Errorea ezabatzean", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            while (LagunakList.SelectedItems.Count > 0)
            {
                LagunakList.Items.Remove(LagunakList.SelectedItems[0]);
            }

            txtHautatuta.Clear();
        }
        private void Garbitu_Click(object sender, RoutedEventArgs e)
        {
            LagunakList.Items.Clear();
            txtHautatuta.Clear();
        }

    private void ListaIzenaAukeratu(object sender, RoutedEventArgs e)
        {
            if (LagunakList.SelectedItem != null)
            {
                txtHautatuta.Text = LagunakList.SelectedItem.ToString();
            }
        }
    }
}
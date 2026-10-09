using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _2._1.Ariketa
{
    public partial class MainWindow : Window
    {
        private List<Ataza> atazak = new();

        public MainWindow()
        {
            InitializeComponent();
            atazak = XmlStorage.Kargatu();
            Eguneratu();
        }

        private void Eguneratu()
        {
            IEnumerable<Ataza> ordenatuta = CmbOrdenazioa.SelectedIndex switch
            {
                1 => atazak.OrderByDescending(a => a.AzkenEguna),
                2 => atazak.OrderBy(a => a.LehentasunaZenb),
                3 => atazak.OrderByDescending(a => a.LehentasunaZenb),
                _ => atazak.OrderBy(a => a.AzkenEguna)
            };
            DgAtazak.ItemsSource = ordenatuta.ToList();
        }

        private void CmbOrdenazioa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgAtazak != null) Eguneratu();
        }

        private void BtnBerria_Click(object sender, RoutedEventArgs e)
        {
            var berria = new Ataza { Id = atazak.Count == 0 ? 1 : atazak.Max(a => a.Id) + 1 };
            var leihoa = new AtazaWindow(berria) { Owner = this };
            if (leihoa.ShowDialog() == true)
            {
                atazak.Add(berria);
                XmlStorage.Gorde(atazak);
                Eguneratu();
            }
        }

        private void BtnEditatu_Click(object sender, RoutedEventArgs e)
        {
            if (DgAtazak.SelectedItem is not Ataza hautatua)
            {
                MessageBox.Show("Hautatu ataza bat editatzeko.", "Oharra");
                return;
            }
            var kopia = new Ataza
            {
                Id = hautatua.Id,
                Titulua = hautatua.Titulua,
                Lehentasuna = hautatua.Lehentasuna,
                AzkenEguna = hautatua.AzkenEguna,
                Egina = hautatua.Egina
            };
            var leihoa = new AtazaWindow(kopia) { Owner = this };
            if (leihoa.ShowDialog() == true)
            {
                hautatua.Titulua = kopia.Titulua;
                hautatua.Lehentasuna = kopia.Lehentasuna;
                hautatua.AzkenEguna = kopia.AzkenEguna;
                hautatua.Egina = kopia.Egina;
                XmlStorage.Gorde(atazak);
                Eguneratu();
            }
        }

        private void BtnEzabatu_Click(object sender, RoutedEventArgs e)
        {
            if (DgAtazak.SelectedItem is not Ataza hautatua)
            {
                MessageBox.Show("Hautatu ataza bat ezabatzeko.", "Oharra");
                return;
            }
            var erantzuna = MessageBox.Show($"\"{hautatua.Titulua}\" ezabatu nahi duzu?",
                "Berrespena", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (erantzuna == MessageBoxResult.Yes)
            {
                atazak.Remove(hautatua);
                XmlStorage.Gorde(atazak);
                Eguneratu();
            }
        }

        private void ChkEgina_Click(object sender, RoutedEventArgs e)
        {
            XmlStorage.Gorde(atazak);
        }

        private void BtnIrten_Click(object sender, RoutedEventArgs e) => Close();
    }
}
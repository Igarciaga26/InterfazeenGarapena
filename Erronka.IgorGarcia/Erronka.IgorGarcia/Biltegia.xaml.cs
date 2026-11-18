using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.Sqlite;

namespace Erronka.IgorGarcia
{
    /// <summary>
    /// Interaction logic for Biltegia.xaml
    /// </summary>
    public partial class Biltegia : Window
    {
        private static readonly CultureInfo Kultura = new CultureInfo("es-ES");
        private Produktua? hautatuta;
        private bool kargatzen = true;

        public Biltegia()
        {
            InitializeComponent();

            // Biltegia ikusteko baimenik gabe: menura itzuli
            if (!Sesioa.BiltegiaIkusi)
            {
                Loaded += (s, e) =>
                {
                    MessageBox.Show("Ez duzu biltegia ikusteko baimenik.", "Biltegia",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    new MainWindow().Show();
                    Close();
                };
                return;
            }

            btnErabiltzaileak.Visibility = Sesioa.Admin ? Visibility.Visible : Visibility.Collapsed;
            btnEzabatu.IsEnabled = Sesioa.Admin;

            try
            {
                List<ProduktuMota> motak = Db.GetMotak();
                cmbMota.ItemsSource = motak;
                cmbMota.SelectedIndex = 0;

                List<ProduktuMota> iragazkiak = Db.GetMotak();
                iragazkiak.Insert(0, new ProduktuMota { Id = 0, Izena = "Denak" });
                cmbIragazkia.ItemsSource = iragazkiak;
                cmbIragazkia.SelectedIndex = 0;

                kargatzen = false;
                Zerrendatu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea datu-basea irakurtzean:\n" + ex.Message, "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ================= LAGUNTZAILEAK =================

        private void Zerrendatu()
        {
            if (kargatzen)
                return;

            try
            {
                List<Produktua> produktuak = Db.GetProduktuakBiltegia();

                if (cmbIragazkia.SelectedItem is ProduktuMota mota && mota.Id != 0)
                    produktuak = produktuak.Where(p => p.MotaId == mota.Id).ToList();

                dgProduktuak.ItemsSource = produktuak;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea datu-basea irakurtzean:\n" + ex.Message, "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FormaGarbitu()
        {
            hautatuta = null;
            dgProduktuak.UnselectAll();
            txtIzena.Text = "";
            txtPrezioa.Text = "";
            txtStock.Text = "";
            cmbMota.SelectedIndex = 0;
        }

        // Formularioko datuak jasotzen ditu. Null = baliogabea.
        private Produktua? Baliozkotu()
        {
            string izena = txtIzena.Text.Trim();

            if (izena == "")
            {
                MessageBox.Show("Idatzi produktuaren izena.", "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIzena.Focus();
                return null;
            }

            if (cmbMota.SelectedValue == null)
            {
                MessageBox.Show("Hautatu produktu mota.", "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            string prezioTestua = txtPrezioa.Text.Trim().Replace('.', ',');
            if (!double.TryParse(prezioTestua, NumberStyles.Number, Kultura, out double prezioa) || prezioa < 0)
            {
                MessageBox.Show("Prezioa ez da zuzena.", "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPrezioa.Focus();
                return null;
            }

            if (!int.TryParse(txtStock.Text.Trim(), out int stock) || stock < 0)
            {
                MessageBox.Show("Stocka zenbaki oso positibo bat izan behar da.", "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtStock.Focus();
                return null;
            }

            return new Produktua
            {
                Izena = izena,
                Prezioa = prezioa,
                Stock = stock,
                MotaId = (int)cmbMota.SelectedValue
            };
        }

        // ================= EKITALDIAK =================

        private void Dg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Stock gutxi dagoenean (5 edo gutxiago), errenkada gorriz
            if (e.Row.Item is Produktua p)
                e.Row.Foreground = p.Stock <= 5
                    ? new SolidColorBrush(Color.FromRgb(0xB4, 0x23, 0x18))
                    : Brushes.Black;
        }

        private void Dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProduktuak.SelectedItem is Produktua p)
            {
                hautatuta = p;
                txtIzena.Text = p.Izena;
                cmbMota.SelectedValue = p.MotaId;
                txtPrezioa.Text = p.Prezioa.ToString("0.00", Kultura);
                txtStock.Text = p.Stock.ToString();
            }
        }

        private void CmbIragazkia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Zerrendatu();
        }

        private void Garbitu_Click(object sender, RoutedEventArgs e)
        {
            FormaGarbitu();
        }

        private void Gorde_Click(object sender, RoutedEventArgs e)
        {
            if (hautatuta != null)
            {
                MessageBox.Show("Produktu bat hautatuta dago. Aldatzeko, erabili 'Aldatu'; berria sortzeko, 'Garbitu'.",
                                "Biltegia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Produktua? berria = Baliozkotu();
            if (berria == null)
                return;

            try
            {
                Db.GordeProduktua(berria);
                MessageBox.Show("Produktua gordeta.", "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                FormaGarbitu();
                Zerrendatu();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                MessageBox.Show("Izen hori duen produktu bat badago jada.", "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea gordetzean:\n" + ex.Message, "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Aldatu_Click(object sender, RoutedEventArgs e)
        {
            if (hautatuta == null)
            {
                MessageBox.Show("Hautatu produktu bat aldatzeko.", "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Produktua? berria = Baliozkotu();
            if (berria == null)
                return;

            berria.Id = hautatuta.Id;

            try
            {
                Db.EguneratuProduktua(berria);
                MessageBox.Show("Produktua aldatuta.", "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                FormaGarbitu();
                Zerrendatu();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                MessageBox.Show("Izen hori duen produktu bat badago jada.", "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea aldatzean:\n" + ex.Message, "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ezabatu_Click(object sender, RoutedEventArgs e)
        {
            if (!Sesioa.Admin)
            {
                MessageBox.Show("Administratzaileek soilik ezabatu dezakete.", "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (hautatuta == null)
            {
                MessageBox.Show("Hautatu produktu bat ezabatzeko.", "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult erantzuna = MessageBox.Show(
                hautatuta.Izena + " produktua ezabatu nahi duzu?",
                "Biltegia", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (erantzuna != MessageBoxResult.Yes)
                return;

            try
            {
                Db.EzabatuProduktua(hautatuta.Id);
                FormaGarbitu();
                Zerrendatu();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                MessageBox.Show("Produktu hau eskaeretan agertzen da eta ezin da ezabatu. Ezarri stocka 0an.",
                                "Biltegia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea ezabatzean:\n" + ex.Message, "Biltegia",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Menua_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

        private void Erreserbak_Click(object sender, RoutedEventArgs e)
        {
            new Erreserbak().Show();
            Close();
        }

        private void Erabiltzaileak_Click(object sender, RoutedEventArgs e)
        {
            new Erabiltzaileak().Show();
            Close();
        }
    }
}
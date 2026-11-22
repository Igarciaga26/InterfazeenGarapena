using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.Sqlite;

namespace Erronka.IgorGarcia
{
    /// <summary>
    /// Interaction logic for Erreserbak.xaml
    /// </summary>
    public partial class Erreserbak : Window
    {
        private List<Mahaia> mahaiak = new List<Mahaia>();
        private List<Erreserba> erreserbak = new List<Erreserba>();
        private Mahaia? hautatuta;
        private Erreserba? hautatutakoErreserba;
        private string txanda = "Bazkaria";
        private bool kargatzen = true;

        public Erreserbak()
        {
            InitializeComponent();

            if (Sesioa.Unekoa == null)
            {
                Loaded += (s, e) =>
                {
                    new MainWindow().Show();
                    Close();
                };
                return;
            }

            NabigazioaEguneratu();

            try
            {
                mahaiak = Db.GetMahaiak();
                dpData.SelectedDate = DateTime.Today;
                TxandaAldatu("Bazkaria");
                kargatzen = false;
                Eguneratu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea datu-basea irakurtzean:\n" + ex.Message, "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void NabigazioaEguneratu()
        {
            btnBiltegia.Visibility = Sesioa.BiltegiaIkusi ? Visibility.Visible : Visibility.Collapsed;
            btnErabiltzaileak.Visibility = Sesioa.Admin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Erabiltzaileak_Click(object sender, RoutedEventArgs e)
        {
            Erabiltzaileak leihoa = new Erabiltzaileak();
            leihoa.Show();
            Close();
        }

        private void Menua_Click(object sender, RoutedEventArgs e)
        {
            MainWindow leihoa = new MainWindow();
            leihoa.Show();
            Close();
        }


        private static Brush Kolorea(string hex)
        {
            return (Brush)new BrushConverter().ConvertFromString(hex)!;
        }

        private string DataTestua()
        {
            return dpData.SelectedDate!.Value.ToString("yyyy-MM-dd");
        }

        private void TxandaAldatu(string berria)
        {
            txanda = berria;

            bool bazkaria = txanda == "Bazkaria";
            btnBazkaria.Background = Kolorea(bazkaria ? "#DBEAFE" : "#E5E7EB");
            btnBazkaria.Foreground = Kolorea(bazkaria ? "#1E5EFF" : "#374151");
            btnAfaria.Background = Kolorea(bazkaria ? "#E5E7EB" : "#DBEAFE");
            btnAfaria.Foreground = Kolorea(bazkaria ? "#374151" : "#1E5EFF");

            cmbOrdua.ItemsSource = bazkaria
                ? new List<string> { "13:00", "13:30", "14:00", "14:30", "15:00" }
                : new List<string> { "20:00", "20:30", "21:00", "21:30" };
        }

        private void FormaGarbitu()
        {
            txtMahaia.Text = "";
            txtIzena.Text = "";
            txtTelefonoa.Text = "";
            txtOharrak.Text = "";
            cmbPertsonak.SelectedIndex = -1;
            cmbOrdua.SelectedIndex = -1;
        }

        private void GuztiaGarbitu()
        {
            hautatuta = null;
            hautatutakoErreserba = null;
            FormaGarbitu();
        }

        private void Eguneratu()
        {
            ugMahaiak.Children.Clear();

            if (dpData.SelectedDate == null)
                return;

            erreserbak = Db.GetErreserbak(DataTestua(), txanda);

            if (hautatuta != null)
                hautatutakoErreserba = erreserbak.FirstOrDefault(r => r.MahaiId == hautatuta.Id);

            foreach (Mahaia m in mahaiak)
            {
                bool erreserbatuta = erreserbak.Any(r => r.MahaiId == m.Id);
                bool aukeratuta = hautatuta != null && hautatuta.Id == m.Id;

                Button botoia = new Button
                {
                    Content = m.Izena + "\n" + m.Edukiera + " lagun",
                    Style = (Style)FindResource("TableButton"),
                    Tag = m
                };

                if (aukeratuta)
                {
                    botoia.Background = Kolorea("#DBEAFE");
                    botoia.BorderBrush = Kolorea("#1E5EFF");
                    botoia.Foreground = Kolorea("#1E40AF");
                }
                else if (erreserbatuta)
                {
                    botoia.Background = Kolorea("#FDECEA");
                    botoia.BorderBrush = Kolorea("#F1A9A0");
                    botoia.Foreground = Kolorea("#B42318");
                }
                else
                {
                    botoia.Background = Kolorea("#E8F5E9");
                    botoia.BorderBrush = Kolorea("#A5D6A7");
                    botoia.Foreground = Kolorea("#2E7D32");
                }

                botoia.Click += Mahaia_Click;
                ugMahaiak.Children.Add(botoia);
            }
        }

        private Erreserba? Baliozkotu()
        {
            if (hautatuta == null)
            {
                MessageBox.Show("Hautatu mahai bat.", "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (dpData.SelectedDate == null)
            {
                MessageBox.Show("Hautatu erreserbaren data.", "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (txtIzena.Text.Trim() == "")
            {
                MessageBox.Show("Idatzi bezeroaren izena.", "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIzena.Focus();
                return null;
            }

            if (cmbPertsonak.SelectedIndex < 0)
            {
                MessageBox.Show("Hautatu pertsona kopurua.", "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            int pertsonak = cmbPertsonak.SelectedIndex + 1;
            if (pertsonak > hautatuta.Edukiera)
            {
                MessageBox.Show(hautatuta.Izena + " mahaiak " + hautatuta.Edukiera +
                                " lagunentzako tokia du.", "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (cmbOrdua.SelectedItem == null)
            {
                MessageBox.Show("Hautatu ordua.", "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            return new Erreserba
            {
                MahaiId = hautatuta.Id,
                Data = DataTestua(),
                Txanda = txanda,
                Ordua = (string)cmbOrdua.SelectedItem,
                Bezeroa = txtIzena.Text.Trim(),
                Telefonoa = txtTelefonoa.Text.Trim(),
                PertsonaKopurua = pertsonak,
                Oharrak = txtOharrak.Text.Trim()
            };
        }


        private void Mahaia_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button botoia && botoia.Tag is Mahaia m)
            {
                hautatuta = m;
                hautatutakoErreserba = erreserbak.FirstOrDefault(r => r.MahaiId == m.Id);

                FormaGarbitu();
                txtMahaia.Text = m.Izena;

                if (hautatutakoErreserba != null)
                {
                    Erreserba r = hautatutakoErreserba;
                    txtIzena.Text = r.Bezeroa;
                    txtTelefonoa.Text = r.Telefonoa;
                    txtOharrak.Text = r.Oharrak;
                    cmbPertsonak.SelectedIndex = r.PertsonaKopurua - 1;
                    cmbOrdua.SelectedItem = r.Ordua;
                }
                else
                {
                    cmbPertsonak.SelectedIndex = Math.Min(4, m.Edukiera) - 1;
                }

                Eguneratu();
            }
        }

        private void DpData_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (kargatzen)
                return;

            GuztiaGarbitu();
            Eguneratu();
        }

        private void Bazkaria_Click(object sender, RoutedEventArgs e)
        {
            if (txanda == "Bazkaria")
                return;

            TxandaAldatu("Bazkaria");
            GuztiaGarbitu();
            Eguneratu();
        }

        private void Afaria_Click(object sender, RoutedEventArgs e)
        {
            if (txanda == "Afaria")
                return;

            TxandaAldatu("Afaria");
            GuztiaGarbitu();
            Eguneratu();
        }

        private void Garbitu_Click(object sender, RoutedEventArgs e)
        {
            GuztiaGarbitu();
            Eguneratu();
        }

        private void Gorde_Click(object sender, RoutedEventArgs e)
        {
            if (hautatutakoErreserba != null)
            {
                MessageBox.Show("Mahai hau jada erreserbatuta dago. Aldatzeko, erabili 'Aldatu'.",
                                "Erreserbak", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Erreserba? berria = Baliozkotu();
            if (berria == null)
                return;

            try
            {
                Db.GordeErreserba(berria);
                MessageBox.Show("Erreserba gordeta.", "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                GuztiaGarbitu();
                Eguneratu();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                MessageBox.Show("Mahai hau jada erreserbatuta dago data eta txanda horretan.",
                                "Erreserbak", MessageBoxButton.OK, MessageBoxImage.Warning);
                Eguneratu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea erreserba gordetzean:\n" + ex.Message, "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Aldatu_Click(object sender, RoutedEventArgs e)
        {
            if (hautatutakoErreserba == null)
            {
                MessageBox.Show("Hautatu erreserbatutako mahai bat aldatzeko.", "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Erreserba? berria = Baliozkotu();
            if (berria == null)
                return;

            berria.Id = hautatutakoErreserba.Id;

            try
            {
                Db.EguneratuErreserba(berria);
                MessageBox.Show("Erreserba aldatuta.", "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                Eguneratu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea erreserba aldatzean:\n" + ex.Message, "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ezabatu_Click(object sender, RoutedEventArgs e)
        {
            if (hautatutakoErreserba == null)
            {
                MessageBox.Show("Hautatu erreserbatutako mahai bat ezabatzeko.", "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult erantzuna = MessageBox.Show(
                hautatutakoErreserba.Bezeroa + " bezeroaren erreserba ezabatu nahi duzu?",
                "Erreserbak", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (erantzuna != MessageBoxResult.Yes)
                return;

            try
            {
                Db.EzabatuErreserba(hautatutakoErreserba.Id);

                GuztiaGarbitu();
                Eguneratu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea erreserba ezabatzean:\n" + ex.Message, "Erreserbak",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Biltegia_Click(object sender, RoutedEventArgs e)
        {
            new Biltegia().Show();
            Close();
        }
    }
}
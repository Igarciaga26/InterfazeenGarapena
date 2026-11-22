using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Erronka.IgorGarcia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<EskaeraLerroa> eskaera = new ObservableCollection<EskaeraLerroa>();
        private static readonly CultureInfo Kultura = new CultureInfo("es-ES");

        private string pantaila = "0";           
        private int kantitatea = 1;              
        private bool kantitateaFinkatuta = false;
        private bool berria = true;             

        public MainWindow()
        {
            InitializeComponent();
            SaioaInterfazeaEguneratu();

            lstEskaera.ItemsSource = eskaera;

            try
            {
                List<ProduktuMota> motak = Db.GetMotak();
                motak.Insert(0, new ProduktuMota { Id = 0, Izena = "Denak" });

                lstMotak.ItemsSource = motak;
                lstMotak.SelectedIndex = 0;

                List<Mahaia> mahaiak = Db.GetMahaiak();
                mahaiak.Insert(0, new Mahaia { Id = 0, Izena = "Barra" });

                cmbMahaia.ItemsSource = mahaiak;
                cmbMahaia.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea zerbitzariarekin komunikatzean:\n" + ex.Message, "TPV Elkartea",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Loaded += (s, e) =>
            {
                if (Sesioa.Unekoa == null)
                    LoginEskatu();
            };
        }


        private void LstMotak_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMotak.SelectedItem is ProduktuMota mota)
            {
                try
                {
                    icProduktuak.ItemsSource = Db.GetProduktuak(mota.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errorea produktuak kargatzean:\n" + ex.Message, "TPV Elkartea",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Produktua_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button botoia && botoia.Tag is Produktua produktua)
            {
                int kopurua = kantitateaFinkatuta ? kantitatea : 1;

                EskaeraLerroa? lerroa = eskaera.FirstOrDefault(l => l.ProduktuId == produktua.Id);

                if (lerroa == null)
                {
                    eskaera.Add(new EskaeraLerroa
                    {
                        ProduktuId = produktua.Id,
                        Izena = produktua.Izena,
                        Prezioa = produktua.Prezioa,
                        Kantitatea = kopurua
                    });
                }
                else
                {
                    lerroa.Kantitatea += kopurua;
                }

                GuztiraEguneratu();
                KalkGarbitu();
            }
        }

        private void LstEskaera_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstEskaera.SelectedItem is EskaeraLerroa lerroa)
            {
                if (lerroa.Kantitatea > 1)
                    lerroa.Kantitatea--;
                else
                    eskaera.Remove(lerroa);

                GuztiraEguneratu();
            }
        }

        private void KenduLerroa_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button botoia && botoia.Tag is EskaeraLerroa lerroa)
            {
                eskaera.Remove(lerroa);
                GuztiraEguneratu();
            }
        }

        private void KenduLerroa_DobleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void GuztiraEguneratu()
        {
            double guztira = eskaera.Sum(l => l.Guztira);
            txtGuztira.Text = guztira.ToString("N2", Kultura) + "€";
        }

        private void Imprimatu_Click(object sender, RoutedEventArgs e)
        {
            Erabiltzailea? saioa = Sesioa.Unekoa;

            if (eskaera.Count == 0)
            {
                MessageBox.Show("Eskaera hutsik dago.", "Imprimatu",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (saioa == null)
            {
                MessageBox.Show("Saioa hasi behar duzu eskaera gordetzeko.", "Imprimatu",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Mahaia mahaia = (Mahaia)cmbMahaia.SelectedItem;
            int? mahaiId = mahaia.Id == 0 ? (int?)null : mahaia.Id;

            try
            {
                int eskaeraId = Db.GordeEskaera(saioa.Id, mahaiId, eskaera);

                StringBuilder tikheta = new StringBuilder();
                tikheta.AppendLine("TPV Elkartea - Eskaera #" + eskaeraId);
                tikheta.AppendLine(mahaia.Izena + " - " + saioa.Izena);
                tikheta.AppendLine("--------------------------------");
                foreach (EskaeraLerroa l in eskaera)
                {
                    tikheta.AppendLine(l.Kantitatea + " x " + l.Izena + "   " + l.Guztira.ToString("N2", Kultura) + "€");
                }
                tikheta.AppendLine("--------------------------------");
                tikheta.AppendLine("Guztira: " + eskaera.Sum(l => l.Guztira).ToString("N2", Kultura) + "€");

                MessageBox.Show(tikheta.ToString(), "Eskaera gordeta",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                eskaera.Clear();
                GuztiraEguneratu();
                cmbMahaia.SelectedIndex = 0;
            }
            catch (InvalidOperationException ex)
            {
                // Stock nahikorik ez
                MessageBox.Show(ex.Message, "Imprimatu",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea eskaera gordetzean:\n" + ex.Message, "Imprimatu",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void KalkGarbitu()
        {
            pantaila = "0";
            kantitatea = 1;
            kantitateaFinkatuta = false;
            berria = true;
            txtPantaila.Text = "0";
        }

        private void Kalk_Click(object sender, RoutedEventArgs e)
        {
            string t = (string)((Button)sender).Content;

            if (t.Length == 1 && char.IsDigit(t[0]))
            {
                kantitateaFinkatuta = false;

                if (berria)
                {
                    pantaila = t;
                    berria = false;
                }
                else if (pantaila.Length < 3)
                {
                    pantaila = pantaila == "0" ? t : pantaila + t;
                }

                txtPantaila.Text = pantaila;
            }
            else if (t == "*")
            {
                if (int.TryParse(pantaila, out int n) && n > 0)
                {
                    kantitatea = n;
                    kantitateaFinkatuta = true;
                    berria = true;
                    txtPantaila.Text = n + " ×";
                }
            }
            else if (t == "CE")
            {
                KalkGarbitu();
            }
        }


        private void LoginEskatu()
        {
            SaioaHasi leihoa = new SaioaHasi();
            leihoa.Owner = this;

            if (leihoa.ShowDialog() == true)
            {
                SaioaInterfazeaEguneratu();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void SaioaInterfazeaEguneratu()
        {
            txtSaioa.Text = Sesioa.Unekoa?.Izena ?? "Saioa hasi";
            NabigazioaEguneratu();
        }

        private void SaioaHasi_Click(object sender, RoutedEventArgs e)
        {
            if (Sesioa.Unekoa == null)
            {
                LoginEskatu();
                return;
            }

            MessageBoxResult erantzuna = MessageBox.Show(
                Sesioa.Unekoa.Izena + ", saioa itxi nahi duzu?",
                "Saioa itxi", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (erantzuna != MessageBoxResult.Yes)
                return;

            try
            {
                Db.Logout();
            }
            catch (Exception)
            {
            }

            Sesioa.Unekoa = null;
            eskaera.Clear();
            GuztiraEguneratu();
            KalkGarbitu();
            cmbMahaia.SelectedIndex = 0;
            SaioaInterfazeaEguneratu();

            LoginEskatu();
        }

        private void NabigazioaEguneratu()
        {
            btnBiltegia.Visibility = Sesioa.BiltegiaIkusi ? Visibility.Visible : Visibility.Collapsed;
            btnErabiltzaileak.Visibility = Sesioa.Admin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Erreserbak_Click(object sender, RoutedEventArgs e)
        {
            new Erreserbak().Show();
            Close();
        }

        private void Biltegia_Click(object sender, RoutedEventArgs e)
        {
            new Biltegia().Show();
            Close();
        }

        private void Erabiltzaileak_Click(object sender, RoutedEventArgs e)
        {
            new Erabiltzaileak().Show();
            Close();
        }
    }
}
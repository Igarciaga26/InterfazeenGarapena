using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace Erronka.IgorGarcia
{
    /// <summary>
    /// Interaction logic for Erabiltzaileak.xaml
    /// </summary>
    public partial class Erabiltzaileak : Window
    {
        private Erabiltzailea? hautatuta;

        public Erabiltzaileak()
        {
            InitializeComponent();

            cmbRola.ItemsSource = new[] { "admin", "langilea" };
            cmbRola.SelectedIndex = 1;

            // Administratzaileak soilik: bestela, menura itzuli
            if (!Sesioa.Admin)
            {
                Loaded += (s, e) =>
                {
                    MessageBox.Show("Administratzaileek soilik sar daitezke hemen.", "Erabiltzaileak",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    new MainWindow().Show();
                    Close();
                };
                return;
            }

            Zerrendatu();
        }

        // ================= LAGUNTZAILEAK =================

        private void Zerrendatu()
        {
            try
            {
                dgErabiltzaileak.ItemsSource = Db.GetErabiltzaileak();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea datu-basea irakurtzean:\n" + ex.Message, "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FormaGarbitu()
        {
            hautatuta = null;
            dgErabiltzaileak.UnselectAll();
            txtIzena.Text = "";
            txtEmaila.Text = "";
            pwdPasahitza.Clear();
            cmbRola.SelectedIndex = 1;
            lblPasahitza.Text = "Pasahitza";
        }

        // Formularioko datuak jasotzen ditu. Null = baliogabea.
        private Erabiltzailea? Baliozkotu(bool berria)
        {
            string izena = txtIzena.Text.Trim();
            string emaila = txtEmaila.Text.Trim();
            string pasahitza = pwdPasahitza.Password;

            if (izena == "")
            {
                MessageBox.Show("Idatzi izena.", "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIzena.Focus();
                return null;
            }

            if (emaila == "" || !emaila.Contains("@"))
            {
                MessageBox.Show("Idatzi baliozko email bat.", "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmaila.Focus();
                return null;
            }

            if (berria && pasahitza == "")
            {
                MessageBox.Show("Idatzi pasahitza.", "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                pwdPasahitza.Focus();
                return null;
            }

            return new Erabiltzailea
            {
                Izena = izena,
                Emaila = emaila,
                Pasahitza = pasahitza,
                Rola = (string)cmbRola.SelectedItem
            };
        }

        // ================= EKITALDIAK =================

        private void Dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgErabiltzaileak.SelectedItem is Erabiltzailea u)
            {
                hautatuta = u;
                txtIzena.Text = u.Izena;
                txtEmaila.Text = u.Emaila;
                cmbRola.SelectedItem = u.Rola;
                pwdPasahitza.Clear();
                lblPasahitza.Text = "Pasahitza berria (hutsik utzi aldatu nahi ez baduzu)";
            }
        }

        private void Garbitu_Click(object sender, RoutedEventArgs e)
        {
            FormaGarbitu();
        }

        private void Gorde_Click(object sender, RoutedEventArgs e)
        {
            if (hautatuta != null)
            {
                MessageBox.Show("Erabiltzaile bat hautatuta dago. Aldatzeko, erabili 'Aldatu'; berria sortzeko, 'Garbitu'.",
                                "Erabiltzaileak", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Erabiltzailea? berria = Baliozkotu(true);
            if (berria == null)
                return;

            try
            {
                Db.GordeErabiltzailea(berria);
                MessageBox.Show("Erabiltzailea gordeta.", "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                FormaGarbitu();
                Zerrendatu();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                MessageBox.Show("Email hori beste erabiltzaile batek dauka.", "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea gordetzean:\n" + ex.Message, "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Aldatu_Click(object sender, RoutedEventArgs e)
        {
            if (hautatuta == null)
            {
                MessageBox.Show("Hautatu erabiltzaile bat aldatzeko.", "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Erabiltzailea? berria = Baliozkotu(false);
            if (berria == null)
                return;

            berria.Id = hautatuta.Id;

            try
            {
                if (hautatuta.Rola == "admin" && berria.Rola != "admin" && Db.AdminKopurua() <= 1)
                {
                    MessageBox.Show("Ezin da azken administratzailearen rola aldatu.", "Erabiltzaileak",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Db.EguneratuErabiltzailea(berria);

                // Norberaren datuak aldatu badira, saioa eguneratu
                if (Sesioa.Unekoa != null && Sesioa.Unekoa.Id == berria.Id)
                {
                    Sesioa.Unekoa.Izena = berria.Izena;
                    Sesioa.Unekoa.Rola = berria.Rola;
                }

                MessageBox.Show("Erabiltzailea aldatuta.", "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                FormaGarbitu();
                Zerrendatu();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                MessageBox.Show("Email hori beste erabiltzaile batek dauka.", "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea aldatzean:\n" + ex.Message, "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ezabatu_Click(object sender, RoutedEventArgs e)
        {
            if (hautatuta == null)
            {
                MessageBox.Show("Hautatu erabiltzaile bat ezabatzeko.", "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Sesioa.Unekoa != null && Sesioa.Unekoa.Id == hautatuta.Id)
            {
                MessageBox.Show("Ezin duzu zure burua ezabatu.", "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (hautatuta.Rola == "admin" && Db.AdminKopurua() <= 1)
                {
                    MessageBox.Show("Ezin da azken administratzailea ezabatu.", "Erabiltzaileak",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult erantzuna = MessageBox.Show(
                    hautatuta.Izena + " erabiltzailea ezabatu nahi duzu?",
                    "Erabiltzaileak", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (erantzuna != MessageBoxResult.Yes)
                    return;

                Db.EzabatuErabiltzailea(hautatuta.Id);
                FormaGarbitu();
                Zerrendatu();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                MessageBox.Show("Erabiltzaile honek eskaerak ditu eta ezin da ezabatu.", "Erabiltzaileak",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errorea ezabatzean:\n" + ex.Message, "Erabiltzaileak",
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
        private void Biltegia_Click(object sender, RoutedEventArgs e)
        {
            new Biltegia().Show();
            Close();
        }
    }
}
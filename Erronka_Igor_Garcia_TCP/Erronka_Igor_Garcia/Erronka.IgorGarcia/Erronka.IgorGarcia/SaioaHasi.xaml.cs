using System;
using System.Windows;

namespace Erronka.IgorGarcia
{
    /// <summary>
    /// Interaction logic for SaioaHasi.xaml
    /// </summary>
    public partial class SaioaHasi : Window
    {
        public Erabiltzailea? Erabiltzailea { get; private set; }

        public SaioaHasi()
        {
            InitializeComponent();
            txtErabiltzailea.Focus();
        }

        private void Sartu_Click(object sender, RoutedEventArgs e)
        {
            string erabiltzailea = txtErabiltzailea.Text.Trim();
            string pasahitza = pwdPasahitza.Password;

            if (erabiltzailea == "" || pasahitza == "")
            {
                MessageBox.Show("Bete eremu guztiak.", "Saioa hasi",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Erabiltzailea = Db.Login(erabiltzailea, pasahitza);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ezin da zerbitzariarekin konektatu:\n" + ex.Message, "Saioa hasi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Erabiltzailea == null)
            {
                MessageBox.Show("Erabiltzailea edo pasahitza okerra.", "Saioa hasi",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                pwdPasahitza.Clear();
                pwdPasahitza.Focus();
                return;
            }

            Sesioa.Unekoa = Erabiltzailea;
            DialogResult = true;
        }
    }
}
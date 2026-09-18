using System;
using System.Windows;
using System.Windows.Controls;

namespace _1.Ariketa.IgorGarcia
{
    public class ZenbakiBaliogabeaException : Exception
    {
        public ZenbakiBaliogabeaException(string mezua) : base(mezua)
        {
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private double ZenbakiaLortu(TextBox zenb, string izena)
        {
            if (!double.TryParse(zenb.Text, out double zenbakia))
                throw new ZenbakiBaliogabeaException(izena + " kutxan ez dago zenbaki baliogarririk.");

            return zenbakia;
        }

        private void Kalkulatu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double zenb1 = ZenbakiaLortu(txtLehenengoZenbakia, "Lehenengo");
                double zenb2 = ZenbakiaLortu(txtBigarrenZenbakia, "Bigarren");
                double zenb3 = ZenbakiaLortu(txtHirugarrenZenbakia, "Hirugarren");
                double zenb4 = ZenbakiaLortu(txtLaugarrenZenbakia, "Laugarren");

                double gehi = zenb1 + zenb2 + zenb3 + zenb4;
                double bb = gehi / 4;

                txtEmaitza.Text = bb.ToString();
            }
            catch (ZenbakiBaliogabeaException ex)
            {
                MessageBox.Show(ex.Message, "Errorea");
            }
        }

        private void Garbitu_Click(object sender, RoutedEventArgs e)
        {
            txtLehenengoZenbakia.Clear();
            txtBigarrenZenbakia.Clear();
            txtHirugarrenZenbakia.Clear();
            txtLaugarrenZenbakia.Clear();
            txtEmaitza.Clear();
        }

        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
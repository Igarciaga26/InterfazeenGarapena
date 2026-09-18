using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace _7.Ariketa.IgorGarcia
{
    public partial class MainWindow : Window
    {
        private double zenb1 = 0;
        private string eragiketaAukeratua = "";
        private bool zenb2 = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ZenbBtn_Click(object sender, RoutedEventArgs e)
        {
            string zenbakia = ((Button)sender).Content.ToString();

            if (txtPantaila.Text == "0," || zenb2)
            {
                txtPantaila.Text = zenbakia;
                zenb2 = false;
            }
            else
            {
                txtPantaila.Text += zenbakia;
            }
        }

        private void KomaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (zenb2)
            {
                txtPantaila.Text = "0,";
                zenb2 = false;
                return;
            }

            if (!txtPantaila.Text.Contains(","))
                txtPantaila.Text += ",";
        }

        private void EragiketaBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                zenb1 = double.Parse(txtPantaila.Text.TrimEnd(','), CultureInfo.CurrentCulture);
                eragiketaAukeratua = ((Button)sender).Content.ToString();
                zenb2 = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Zenbaki baliogabea.", "Errorea");
                C_Click(sender, e);
            }
        }

        private void BerdinBtn_Click(object sender, RoutedEventArgs e)
        {
            Kalkulatu();
        }

        private void EhunekoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double emaitza = double.Parse(txtPantaila.Text.TrimEnd(','), CultureInfo.CurrentCulture) / 100.0;
                txtPantaila.Text = emaitza + ",";
                zenb2 = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Zenbaki baliogabea.", "Errorea");
            }
        }

        private void C_Click(object sender, RoutedEventArgs e)
        {
            txtPantaila.Text = "0,";
            zenb1 = 0;
            eragiketaAukeratua = "";
            zenb2 = true;
        }

        private void CE_Click(object sender, RoutedEventArgs e)
        {
            txtPantaila.Text = "0,";
            zenb2 = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                Kalkulatu();
        }

        private void Kalkulatu()
        {
            if (string.IsNullOrEmpty(eragiketaAukeratua))
                return;

            try
            {
                double bigarrenZenbakia = double.Parse(txtPantaila.Text.TrimEnd(','), CultureInfo.CurrentCulture);
                double emaitza;

                switch (eragiketaAukeratua)
                {
                    case "+": emaitza = zenb1 + bigarrenZenbakia; break;
                    case "-": emaitza = zenb1 - bigarrenZenbakia; break;
                    case "×": emaitza = zenb1 * bigarrenZenbakia; break;
                    case "/": emaitza = zenb1 / bigarrenZenbakia; break;
                    default: return;
                }

                txtPantaila.Text = emaitza+"";
            }
            catch (Exception)
            {
                MessageBox.Show("Zenbakia ez du balio.", "Errorea");
            }
            finally
            {
                eragiketaAukeratua = "";
                zenb2 = true;
            }
        }
    }
}
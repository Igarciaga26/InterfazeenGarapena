using AriketaMultzoa3;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _11.Ariketa.IgorGarcia
{
    /// <summary>
    /// Interaction logic for Bistaratu.xaml
    /// </summary>
    public partial class Bistaratu : Window
    {
        public Bistaratu()
        {
            InitializeComponent();
            lblIzena.Content = Datuak.Izena;
            lblAbizena1.Content = Datuak.Abizena1;
            lblAbizena2.Content = Datuak.Abizena2;
            lblNan.Content = Datuak.NAN;
        }
        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

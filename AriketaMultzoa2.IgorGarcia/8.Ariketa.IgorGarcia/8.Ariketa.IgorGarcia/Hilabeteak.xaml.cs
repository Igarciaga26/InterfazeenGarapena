using System;
using System.Windows;

namespace _8.Ariketa.IgorGarcia
{
    public partial class Hilabeteak : Window
    {
        public int? Kopurua { get; private set; }

        public Hilabeteak()
        {
            InitializeComponent();
        }

        private void Onartu_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtHilabeteKop.Text, out int kopurua))
            {
                Kopurua = kopurua;
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
        }

        private void Utzi_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
using System;
using System.Windows;
using System.Globalization;

namespace _8.Ariketa.IgorGarcia
{
    public partial class DatenAldea : Window
    {
        public DateTime? Data { get; private set; }

        public DatenAldea(string titulua, string mezua)
        {
            InitializeComponent();
            Title = titulua;
            Mezualbl.Content = mezua;
        }

        private void Onartu_Click(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParseExact(DataTextBox.Text, "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data))
            {
                Data = data;
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

        private void DataTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
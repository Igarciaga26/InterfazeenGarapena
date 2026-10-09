using System;
using System.Windows;
using System.Windows.Controls;

namespace _2._1.Ariketa
{
    public partial class AtazaWindow : Window
    {
        private readonly Ataza ataza;

        public AtazaWindow(Ataza ataza)
        {
            InitializeComponent();
            this.ataza = ataza;

            TxtTitulua.Text = ataza.Titulua;
            DpAzkenEguna.SelectedDate = ataza.AzkenEguna;
            ChkEgina.IsChecked = ataza.Egina;
            foreach (ComboBoxItem item in CmbLehentasuna.Items)
                if ((string)item.Content == ataza.Lehentasuna)
                    CmbLehentasuna.SelectedItem = item;
            if (CmbLehentasuna.SelectedItem == null) CmbLehentasuna.SelectedIndex = 1;
        }

        private void BtnOnartu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitulua.Text))
            {
                MessageBox.Show("Izenburua derrigorrezkoa da.", "Errorea",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (DpAzkenEguna.SelectedDate is not DateTime data || data.Date < DateTime.Today)
            {
                MessageBox.Show("Muga-eguna gaur edo geroagokoa izan behar da.", "Errorea",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ataza.Titulua = TxtTitulua.Text.Trim();
            ataza.Lehentasuna = (string)((ComboBoxItem)CmbLehentasuna.SelectedItem).Content;
            ataza.AzkenEguna = data.Date;
            ataza.Egina = ChkEgina.IsChecked == true;
            DialogResult = true;
        }
    }
}
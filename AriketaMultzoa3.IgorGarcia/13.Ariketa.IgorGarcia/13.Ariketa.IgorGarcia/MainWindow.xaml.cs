using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _13.Ariketa.IgorGarcia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Moztu_Click(object sender, RoutedEventArgs e)
        {
            if (txtEditor.SelectionLength > 0)
            {
                Clipboard.SetText(txtEditor.SelectedText);
                txtEditor.SelectedText = "";
            }
            txtEditor.Focus();
        }

        private void Kopiatu_Click(object sender, RoutedEventArgs e)
        {
            if (txtEditor.SelectionLength > 0)
            {
                Clipboard.SetText(txtEditor.SelectedText);
            }
            txtEditor.Focus();
        }
        private void Itsatsi_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                txtEditor.SelectedText = Clipboard.GetText();
                txtEditor.CaretIndex += txtEditor.SelectionLength;
            }
            txtEditor.Focus();
        }

        private void Ezabatu_Click(object sender, RoutedEventArgs e)
        {
            txtEditor.SelectedText = "";
            txtEditor.Focus();
        }
        private void Iturria_Click(object sender, RoutedEventArgs e)
        {
            MenuItem aukera = (MenuItem)sender;
            txtEditor.FontFamily = new FontFamily(aukera.Header.ToString());
            txtEditor.Focus();
        }
    }
}
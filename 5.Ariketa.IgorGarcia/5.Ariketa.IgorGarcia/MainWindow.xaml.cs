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

namespace _5.Ariketa.IgorGarcia
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

        private void Comic_Click(object sender, RoutedEventArgs e)
        {
            txtTextua.FontFamily = new FontFamily("Comic Sans MS");
        }
        private void Courier_Click(object sender, RoutedEventArgs e)
        {
            txtTextua.FontFamily = new FontFamily("Courier New");
        }
        private void Lodia_Click(object sender, RoutedEventArgs e)
        {
            if(txtTextua.FontWeight == FontWeights.Bold)
            {
                txtTextua.FontWeight = FontWeights.Normal;
            }
            else
            {
                txtTextua.FontWeight = FontWeights.Bold;
            }
        }
        private void Etzana_Click(object sender, RoutedEventArgs e)
        {
            if (txtTextua.FontStyle == FontStyles.Italic)
            {
                txtTextua.FontStyle = FontStyles.Normal;
            }
            else
            {
                txtTextua.FontStyle = FontStyles.Italic;
            }
        }
        private void Azpimarratua_Click(object sender, RoutedEventArgs e)
        {
            if (txtTextua.TextDecorations == TextDecorations.Underline)
            {
                txtTextua.TextDecorations = null;
            }
            else
            {
                txtTextua.TextDecorations = TextDecorations.Underline;
            }
        }
        private void Marratua_Click(object sender, RoutedEventArgs e)
        {
            if (txtTextua.TextDecorations == TextDecorations.Strikethrough)
                txtTextua.TextDecorations = null;
            else
                txtTextua.TextDecorations = TextDecorations.Strikethrough;
        }
        private void PlusTamaina_Click(object sender, RoutedEventArgs e)
        {
            txtTextua.FontSize += 2;
        }
        private void MinusTamaina_Click(object sender, RoutedEventArgs e)
        {
            txtTextua.FontSize -= 2;
        }
        private void Hautatu_Click(object sender, RoutedEventArgs e)
        {
            int hautatutakoTextua = txtTextua.Text.Length;
            string hautatutakoa = txtTextua.SelectedText;

            TxtEmaitza.Text = $"Testuak {hautatutakoTextua} karaktere ditu eta hautatutako testua hau da: {hautatutakoa}";
        }
    }
}
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

namespace _6.Ariketa.IgorGarcia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Testu_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                TextBox textua = sender as TextBox;

                if(textua != null)
                {
                    if(textua==txtText1)
                    {
                        txtText2.Text = txtText1.Text;
                        txtText1.Clear();
                        txtText2.Focus();
                    }
                    else if (textua == txtText2)
                    {
                        txtText3.Text = txtText2.Text;
                        txtText2.Clear();
                        txtText3.Focus();
                    }
                    else if (textua == txtText3)
                    {
                        txtText1.Text = txtText3.Text;
                        txtText3.Clear();
                        txtText1.Focus();
                    }
                }
            }
        }

        private void Garbitu_Click(object sender, RoutedEventArgs e)
        {
            txtText1.Clear();
            txtText2.Clear();
            txtText3.Clear();
        }

        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
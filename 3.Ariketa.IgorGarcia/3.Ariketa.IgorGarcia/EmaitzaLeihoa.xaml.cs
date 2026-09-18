using System.Windows;

namespace _3.Ariketa.IgorGarcia
{
    public partial class EmaitzaLeihoa : Window
    {
        public bool GarbituNahiDa { get; private set; } = false;

        public EmaitzaLeihoa(double emaitza)
        {
            InitializeComponent();
            Txtresultado.Text = emaitza.ToString("F2");
        }

        private void Garbitu_Click(object sender, RoutedEventArgs e)
        {
            GarbituNahiDa = true;
            this.Close();
        }

        private void Irten_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
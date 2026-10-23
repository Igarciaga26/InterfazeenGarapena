using System.Windows;
using System.Windows.Controls;

namespace _3._1.IgorGarcia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GelaxkakSortu(4, 10); 
        }

        private void GelaxkakSortu(int zutabeak, int ilarak)
        {
            matrizeaGrid.Children.Clear();
            matrizeaGrid.ColumnDefinitions.Clear();
            matrizeaGrid.RowDefinitions.Clear();

            for (int i = 0; i < zutabeak; i++)
                matrizeaGrid.ColumnDefinitions.Add(
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < ilarak; i++)
                matrizeaGrid.RowDefinitions.Add(
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            for (int z = 0; z < zutabeak; z++)
            {
                for (int i = 0; i < ilarak; i++)
                {
                    var g = new GelaxkaKontrola();
                    Grid.SetColumn(g, z);
                    Grid.SetRow(g, i);
                    matrizeaGrid.Children.Add(g);
                }
            }
        }
    }
}
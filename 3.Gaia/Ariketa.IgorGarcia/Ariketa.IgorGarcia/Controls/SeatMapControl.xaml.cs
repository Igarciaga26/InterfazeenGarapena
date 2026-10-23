using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Ariketa.IgorGarcia;

/// <summary>Lekuaren egoera: librea, hautatua (oraindik erreserbatu gabe) edo okupatua (erreserbatua).</summary>
public enum SeatState { Free, Selected, Occupied }

/// <summary>Gertaeren datuak: zein lekutan egin den klik (indizeak 0-tik hasten dira).</summary>
public class SeatEventArgs : EventArgs
{
    public SeatEventArgs(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public int Row { get; }
    public int Column { get; }
}

/// <summary>Leku-mapa berrerabilgarria. Konfigurazioa propietateen bidez egiten da.</summary>
public partial class SeatMapControl : UserControl
{
    private static readonly Color FreeColor = Color.FromRgb(0x22, 0xC5, 0x5E);     // berdea
    private static readonly Color OccupiedColor = Color.FromRgb(0x9C, 0xA3, 0xAF); // grisa

    private Border[,] _borders = new Border[0, 0];
    private SeatState[,] _states = new SeatState[0, 0];

    // ---------------- Propietateak ----------------

    public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(
        nameof(Rows), typeof(int), typeof(SeatMapControl),
        new PropertyMetadata(5, (d, e) => ((SeatMapControl)d).Rebuild()));

    public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
        nameof(Columns), typeof(int), typeof(SeatMapControl),
        new PropertyMetadata(4, (d, e) => ((SeatMapControl)d).Rebuild()));

    public static readonly DependencyProperty AccentColorProperty = DependencyProperty.Register(
        nameof(AccentColor), typeof(Color), typeof(SeatMapControl),
        new PropertyMetadata(Color.FromRgb(0xF9, 0x73, 0x16)));

    /// <summary>Errenkada kopurua. Lehenetsia: 5.</summary>
    public int Rows
    {
        get => (int)GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    /// <summary>Errenkada bakoitzeko eserleku kopurua (gehienez 26). Lehenetsia: 4.</summary>
    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    /// <summary>Hautatutako lekuen kolorea eta hover ertzarena. Lehenetsia: laranja.</summary>
    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    // ---------------- Gertaerak ----------------

    public event EventHandler<SeatEventArgs>? SeatSelected;
    public event EventHandler<SeatEventArgs>? SeatUnselected;
    public event EventHandler<SeatEventArgs>? OccupiedSeatClicked;

    public SeatMapControl()
    {
        InitializeComponent();
        Rebuild();
    }

    // ---------------- Metodo publikoak ----------------

    /// <summary>Lekuaren izena: errenkada zenbakia + zutabe letra, adibidez "3B".</summary>
    public static string GetSeatLabel(int row, int column) => $"{row + 1}{(char)('A' + column)}";

    /// <summary>Hautatutako lekuak erreserbatzen ditu (okupatu bihurtzen dira) eta erreserbatutakoen zerrenda itzultzen du.</summary>
    public List<(int Row, int Column)> Reserve()
    {
        var reserved = new List<(int Row, int Column)>();
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (_states[r, c] != SeatState.Selected) continue;
                SetState(r, c, SeatState.Occupied);
                reserved.Add((r, c));
            }
        }
        return reserved;
    }

    /// <summary>Leku okupatu baten erreserba bertan behera uzten du (libre geratzen da).</summary>
    public void CancelReservation(int row, int column)
    {
        if (_states[row, column] == SeatState.Occupied)
            SetState(row, column, SeatState.Free);
    }

    /// <summary>Leku okupatuak ezartzen ditu; gainerakoak libre geratzen dira.</summary>
    public void SetOccupiedSeats(IEnumerable<(int Row, int Column)> occupied)
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Columns; c++)
                SetState(r, c, SeatState.Free);

        foreach (var (row, column) in occupied)
        {
            if (row < Rows && column < Columns)
                SetState(row, column, SeatState.Occupied);
        }
    }

    // ---------------- Barne-logika ----------------

    private void Rebuild()
    {
        SeatGrid.Children.Clear();
        SeatGrid.Rows = Rows;
        SeatGrid.Columns = Columns;
        _borders = new Border[Rows, Columns];
        _states = new SeatState[Rows, Columns];

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                int row = r, column = c;

                var border = new Border
                {
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness(3),
                    CornerRadius = new CornerRadius(8),
                    BorderThickness = new Thickness(2),
                    BorderBrush = Brushes.Transparent,
                    Background = new SolidColorBrush(FreeColor),
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    Cursor = Cursors.Hand,
                    Child = new TextBlock
                    {
                        Text = GetSeatLabel(row, column),
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.SemiBold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };

                // Hover: lekua pixka bat handitzen da eta ertza koloreztatzen da
                border.MouseEnter += (_, _) =>
                {
                    border.BorderBrush = new SolidColorBrush(AccentColor);
                    border.RenderTransform = new ScaleTransform(1.1, 1.1);
                };
                border.MouseLeave += (_, _) =>
                {
                    border.BorderBrush = Brushes.Transparent;
                    border.RenderTransform = Transform.Identity;
                };
                border.MouseLeftButtonDown += (_, _) => OnSeatClicked(row, column);

                _borders[r, c] = border;
                SeatGrid.Children.Add(border);
            }
        }
    }

    private void OnSeatClicked(int row, int column)
    {
        var args = new SeatEventArgs(row, column);

        switch (_states[row, column])
        {
            case SeatState.Free:
                SetState(row, column, SeatState.Selected);
                SeatSelected?.Invoke(this, args);
                break;
            case SeatState.Selected:
                SetState(row, column, SeatState.Free);
                SeatUnselected?.Invoke(this, args);
                break;
            default:
                OccupiedSeatClicked?.Invoke(this, args);
                break;
        }
    }

    /// <summary>Lekuaren egoera aldatzen du eta kolorea trantsizio batekin eguneratzen du.</summary>
    private void SetState(int row, int column, SeatState state)
    {
        _states[row, column] = state;

        var color = state switch
        {
            SeatState.Selected => AccentColor,
            SeatState.Occupied => OccupiedColor,
            _ => FreeColor
        };

        var brush = (SolidColorBrush)_borders[row, column].Background;
        brush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(color, TimeSpan.FromMilliseconds(200)));
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ariketa.IgorGarcia;

public partial class MainWindow : Window
{
    private readonly List<ReservationRecord> _reservations = ReservationStorage.Load();

    private TabItem ActiveTab => (TabItem)ZoneTabs.SelectedItem;
    private SeatMapControl ActiveMap => (SeatMapControl)ActiveTab.Content;
    private string ActiveZone => (string)ActiveTab.Header;

    public MainWindow()
    {
        InitializeComponent();
        DateBox.SelectedDate = DateTime.Today.AddDays(1); 
    }

    private void DateBox_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        foreach (TabItem tab in ZoneTabs.Items)
        {
            var zone = (string)tab.Header;
            var occupied = _reservations
                .Where(r => r.Zone == zone && r.Date == DateBox.SelectedDate)
                .Select(r => (r.Row, r.Column));

            ((SeatMapControl)tab.Content).SetOccupiedSeats(occupied);
        }
    }

    private void Map_SeatSelected(object? sender, SeatEventArgs e) =>
        ShowMessage($"{SeatMapControl.GetSeatLabel(e.Row, e.Column)} lekua hautatu da.", isError: false);

    private void Map_SeatUnselected(object? sender, SeatEventArgs e) =>
        ShowMessage($"{SeatMapControl.GetSeatLabel(e.Row, e.Column)} lekua desautatu da.", isError: false);

    private void Map_OccupiedSeatClicked(object? sender, SeatEventArgs e)
    {
        var label = SeatMapControl.GetSeatLabel(e.Row, e.Column);
        var answer = MessageBox.Show($"{label} lekuaren erreserba bertan behera utzi nahi duzu?",
            "Erreserba bertan behera utzi", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes) return;

        ActiveMap.CancelReservation(e.Row, e.Column);
        _reservations.RemoveAll(r => r.Zone == ActiveZone && r.Date == DateBox.SelectedDate
                                     && r.Row == e.Row && r.Column == e.Column);
        ReservationStorage.Save(_reservations);

        ShowMessage($"{label} lekuaren erreserba bertan behera utzi da.", isError: false);
    }

    private void ReserveButton_Click(object sender, RoutedEventArgs e)
    {
        if (DateBox.SelectedDate is not DateTime date || date.Date <= DateTime.Today)
        {
            ShowMessage("Aukeratu etorkizuneko data bat (gaurkoa baino geroagokoa).", isError: true);
            return;
        }

        var reserved = ActiveMap.Reserve();
        if (reserved.Count == 0)
        {
            ShowMessage("Aukeratu gutxienez leku bat erreserbatzeko.", isError: true);
            return;
        }

        foreach (var (row, column) in reserved)
        {
            _reservations.Add(new ReservationRecord { Zone = ActiveZone, Date = date.Date, Row = row, Column = column });
        }
        ReservationStorage.Save(_reservations);

        var labels = string.Join(", ", reserved.Select(s => SeatMapControl.GetSeatLabel(s.Row, s.Column)));
        ShowMessage($"Erreserbatuta ({ActiveZone}, {date:yyyy-MM-dd}): {labels}", isError: false);
    }

    private void ShowMessage(string text, bool isError)
    {
        MessageText.Text = text;
        MessageText.Foreground = isError ? Brushes.Firebrick : Brushes.SeaGreen;
    }
}
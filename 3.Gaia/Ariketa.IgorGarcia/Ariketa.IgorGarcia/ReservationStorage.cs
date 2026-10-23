using System.IO;
using System.Text.Json;

namespace Ariketa.IgorGarcia;

public static class ReservationStorage
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SeatReservation",
        "reservations.json");

    public static List<ReservationRecord> Load()
    {
        if (!File.Exists(FilePath))
            return new List<ReservationRecord>();

        try
        {
            return JsonSerializer.Deserialize<List<ReservationRecord>>(File.ReadAllText(FilePath))
                   ?? new List<ReservationRecord>();
        }
        catch (JsonException)
        {
            return new List<ReservationRecord>(); 
        }
    }

    public static void Save(List<ReservationRecord> reservations)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(reservations));
    }
}
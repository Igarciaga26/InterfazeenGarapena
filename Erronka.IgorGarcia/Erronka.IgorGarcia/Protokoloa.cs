using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Erronka.IgorGarcia
{
    // ================= EREDUAK =================

    public class Erabiltzailea
    {
        public int Id { get; set; }
        public string Izena { get; set; } = "";
        public string Emaila { get; set; } = "";
        public string Pasahitza { get; set; } = "";
        public string Rola { get; set; } = "";
    }

    public class ProduktuMota
    {
        public int Id { get; set; }
        public string Izena { get; set; } = "";

        // Motaren ikonoa (izenaren arabera)
        [JsonIgnore]
        public string Ikonoa
        {
            get
            {
                return Izena switch
                {
                    "Denak" => "🍽",
                    "Hasierakoak" => "🥗",
                    "Okelak" => "🥩",
                    "Arrainak" => "🐟",
                    "Edariak" => "🥤",
                    "Postreak" => "🍰",
                    _ => "🍴"
                };
            }
        }
    }

    public class Produktua
    {
        public int Id { get; set; }
        public string Izena { get; set; } = "";
        public double Prezioa { get; set; }
        public int Stock { get; set; }
        public int MotaId { get; set; }
        public string MotaIzena { get; set; } = "";
        public string? Irudia { get; set; }

        // Irudiaren bide osoa (null = irudirik ez edo fitxategia ez dago)
        [JsonIgnore]
        public string? IrudiBidea
        {
            get
            {
                if (string.IsNullOrEmpty(Irudia))
                    return null;

                string bidea = Path.Combine(AppContext.BaseDirectory, "Irudiak", Irudia);
                return File.Exists(bidea) ? bidea : null;
            }
        }
    }

    public class Mahaia
    {
        public int Id { get; set; }
        public int Zenbakia { get; set; }
        public int Edukiera { get; set; }
        public string Izena { get; set; } = "";
    }

    public class Erreserba
    {
        public int Id { get; set; }
        public int MahaiId { get; set; }
        public string Data { get; set; } = "";
        public string Txanda { get; set; } = "";
        public string Ordua { get; set; } = "";
        public string Bezeroa { get; set; } = "";
        public string Telefonoa { get; set; } = "";
        public int PertsonaKopurua { get; set; }
        public string Oharrak { get; set; } = "";
    }

    public class EskaeraLerroa : INotifyPropertyChanged
    {
        private int kantitatea;

        public int ProduktuId { get; set; }
        public string Izena { get; set; } = "";
        public double Prezioa { get; set; }

        public int Kantitatea
        {
            get { return kantitatea; }
            set
            {
                kantitatea = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Kantitatea)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Guztira)));
            }
        }

        [JsonIgnore]
        public double Guztira
        {
            get { return Prezioa * Kantitatea; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    // ================= PROTOKOLOA =================

    // Bezeroak zerbitzariari bidaltzen diona (JSON lerro bat)
    public class Eskakizuna
    {
        public string Agindua { get; set; } = "";
        public JsonElement? Datuak { get; set; }
    }

    // Zerbitzariak bezeroari erantzuten diona (JSON lerro bat)
    public class Erantzuna
    {
        public bool Ondo { get; set; }
        public string Kodea { get; set; } = "";
        public string Mezua { get; set; } = "";
        public JsonElement? Datuak { get; set; }

        public static Erantzuna Zuzena(object? datuak = null)
        {
            return new Erantzuna
            {
                Ondo = true,
                Datuak = datuak == null ? null : JsonSerializer.SerializeToElement(datuak)
            };
        }

        public static Erantzuna Errorea(string kodea, string mezua)
        {
            return new Erantzuna { Ondo = false, Kodea = kodea, Mezua = mezua };
        }
    }
}
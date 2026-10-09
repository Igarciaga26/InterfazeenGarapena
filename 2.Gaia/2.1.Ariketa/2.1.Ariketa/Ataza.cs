using System;

namespace _2._1.Ariketa
{
    public class Ataza
    {
        public int Id { get; set; }
        public string Titulua { get; set; } = "";
        public string Lehentasuna { get; set; } = "Ertaina"; 
        public DateTime AzkenEguna { get; set; } = DateTime.Today;
        public bool Egina { get; set; }

        public int LehentasunaZenb => Lehentasuna switch
        {
            "Baxua" => 0,
            "Ertaina" => 1,
            _ => 2
        };
    }
}
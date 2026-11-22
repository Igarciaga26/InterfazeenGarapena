using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace Erronka.IgorGarcia
{
    public static class Db
    {
        private static readonly object Blokeoa = new object();
        private static TcpClient? tcp;
        private static StreamReader? irakurlea;
        private static StreamWriter? idazlea;

        // ================= KONEXIOA =================

        // Lehen aldiz behar denean konektatzen da; gero konexio bera erabiltzen du.
        private static void Konektatu()
        {
            try
            {
                tcp = new TcpClient(Protokoloa.Helbidea, Protokoloa.Portua);
                tcp.ReceiveTimeout = 15000;

                NetworkStream stream = tcp.GetStream();
                irakurlea = new StreamReader(stream);
                idazlea = new StreamWriter(stream) { AutoFlush = true };
            }
            catch (SocketException)
            {
                Itxi();
                throw new Exception("Ezin da zerbitzarira konektatu. Zerbitzaria martxan dago?");
            }
        }

        private static void Itxi()
        {
            try { tcp?.Close(); } catch { }
            tcp = null;
            irakurlea = null;
            idazlea = null;
        }

        // Eskakizun bat bidali (JSON lerro bat) eta erantzuna jaso (beste JSON lerro bat)
        private static Erantzuna Bidali(string agindua, object? datuak = null)
        {
            lock (Blokeoa)
            {
                if (tcp == null)
                    Konektatu();

                try
                {
                    Eskakizuna eskakizuna = new Eskakizuna
                    {
                        Agindua = agindua,
                        Datuak = datuak == null ? null : JsonSerializer.SerializeToElement(datuak)
                    };

                    idazlea!.WriteLine(JsonSerializer.Serialize(eskakizuna));

                    string? lerroa = irakurlea!.ReadLine();
                    if (lerroa == null)
                        throw new IOException("Zerbitzariak konexioa itxi du.");

                    Erantzuna erantzuna = JsonSerializer.Deserialize<Erantzuna>(lerroa)!;

                    // Zerbitzaria beteta badago, deskonektatu egiten da; hurrengo deiak berriro konektatuko du
                    if (erantzuna.Kodea == "BETETA")
                        Itxi();

                    return erantzuna;
                }
                catch (IOException ex)
                {
                    Itxi();
                    throw new Exception("Konexioa galdu da zerbitzariarekin: " + ex.Message);
                }
            }
        }

        // Zerbitzariaren errorea bada, dagokion salbuespena jaurtitzen du
        private static void Egiaztatu(Erantzuna e)
        {
            if (e.Ondo)
                return;

            switch (e.Kodea)
            {
                case "KONTRAESTRIKZIOA": throw new SqliteException(e.Mezua, 19);
                case "STOCK": throw new InvalidOperationException(e.Mezua);
                case "EZ_BAIMENIK": throw new UnauthorizedAccessException(e.Mezua);
                default: throw new Exception(e.Mezua);
            }
        }

        private static T Eskatu<T>(string agindua, object? datuak = null)
        {
            Erantzuna e = Bidali(agindua, datuak);
            Egiaztatu(e);
            return e.Datuak!.Value.Deserialize<T>()!;
        }

        private static void Egin(string agindua, object? datuak = null)
        {
            Egiaztatu(Bidali(agindua, datuak));
        }

        // ================= LOGIN =================

        // Erabiltzailea/emaila + pasahitza egiaztatu. Null itzultzen du okerra bada.
        public static Erabiltzailea? Login(string erabiltzailea, string pasahitza)
        {
            Erantzuna e = Bidali("Login", new { Erabiltzailea = erabiltzailea, Pasahitza = pasahitza });

            if (!e.Ondo && e.Kodea == "LOGIN_OKERRA")
                return null;

            Egiaztatu(e);
            return e.Datuak!.Value.Deserialize<Erabiltzailea>();
        }

        public static void Logout()
        {
            Egin("Logout");
        }

        // ================= PRODUKTUAK =================

        public static List<ProduktuMota> GetMotak()
        {
            return Eskatu<List<ProduktuMota>>("GetMotak");
        }

        // motaId = 0 bada, produktu guztiak itzultzen ditu.
        public static List<Produktua> GetProduktuak(int motaId = 0)
        {
            return Eskatu<List<Produktua>>("GetProduktuak", new { MotaId = motaId });
        }

        public static List<Produktua> GetProduktuakBiltegia()
        {
            return Eskatu<List<Produktua>>("GetProduktuakBiltegia");
        }

        public static void GordeProduktua(Produktua p)
        {
            Egin("GordeProduktua", p);
        }

        public static void EguneratuProduktua(Produktua p)
        {
            Egin("EguneratuProduktua", p);
        }

        public static void EzabatuProduktua(int id)
        {
            Egin("EzabatuProduktua", new { Id = id });
        }

        // ================= MAHAIAK ETA ERRESERBAK =================

        public static List<Mahaia> GetMahaiak()
        {
            return Eskatu<List<Mahaia>>("GetMahaiak");
        }

        public static List<Erreserba> GetErreserbak(string data, string txanda)
        {
            return Eskatu<List<Erreserba>>("GetErreserbak", new { Data = data, Txanda = txanda });
        }

        public static void GordeErreserba(Erreserba e)
        {
            Egin("GordeErreserba", e);
        }

        public static void EguneratuErreserba(Erreserba e)
        {
            Egin("EguneratuErreserba", e);
        }

        public static void EzabatuErreserba(int id)
        {
            Egin("EzabatuErreserba", new { Id = id });
        }

        // ================= ESKAERAK =================

        // Eskaera gorde eta stock-a kendu. Eskaeraren id-a itzultzen du.
        // Erabiltzailea zerbitzariak saiotik hartzen du (erabiltzaileId parametroa ez da erabiltzen).
        public static int GordeEskaera(int erabiltzaileId, int? mahaiId, IEnumerable<EskaeraLerroa> lerroak)
        {
            return Eskatu<int>("GordeEskaera", new { MahaiId = mahaiId, Lerroak = lerroak.ToList() });
        }

        // ================= ERABILTZAILEAK =================

        public static List<Erabiltzailea> GetErabiltzaileak()
        {
            return Eskatu<List<Erabiltzailea>>("GetErabiltzaileak");
        }

        public static int AdminKopurua()
        {
            return Eskatu<int>("AdminKopurua");
        }

        public static void GordeErabiltzailea(Erabiltzailea e)
        {
            Egin("GordeErabiltzailea", e);
        }

        public static void EguneratuErabiltzailea(Erabiltzailea e)
        {
            Egin("EguneratuErabiltzailea", e);
        }

        public static void EzabatuErabiltzailea(int id)
        {
            Egin("EzabatuErabiltzailea", new { Id = id });
        }
    }
}
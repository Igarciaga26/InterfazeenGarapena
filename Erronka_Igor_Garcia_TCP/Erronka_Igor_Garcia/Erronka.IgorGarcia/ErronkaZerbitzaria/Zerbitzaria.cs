using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Erronka.IgorGarcia;
using Microsoft.Data.Sqlite;

namespace Erronka.Zerbitzaria
{
    // Konexio bakoitzaren saioa (nork hasi duen saioa)
    public class BezeroEgoera
    {
        public Erabiltzailea? Erabiltzailea { get; set; }
    }

    public class Zerbitzaria
    {
        private const int GehienezkoBezeroak = 10;
        private int _bezeroKopurua = 0;

        // Entzuten geratzen da; datorren bezero bakoitzak bere haria du.
        public void Hasi()
        {
            TcpListener entzulea = new TcpListener(IPAddress.Any, Protokoloa.Portua);
            entzulea.Start();
            Console.WriteLine("Zerbitzaria entzuten: " + Protokoloa.Portua + " portuan.");

            while (true)
            {
                TcpClient bezeroa = entzulea.AcceptTcpClient();

                Thread haria = new Thread(BezeroaKudeatu);
                haria.IsBackground = true;
                haria.Start(bezeroa);
            }
        }

        // Bezero bakoitzaren hariak egiten duena: lerro bat irakurri, lerro bat erantzun, eta berriro.
        private void BezeroaKudeatu(object? parametroa)
        {
            TcpClient? bezeroa = parametroa as TcpClient;
            if (bezeroa == null) return;

            int kopurua = Interlocked.Increment(ref _bezeroKopurua);
            bool beteta = kopurua > GehienezkoBezeroak;
            Console.WriteLine("Bezero bat konektatu da. Aktiboak: " + kopurua);

            try
            {
                using (bezeroa)
                {
                    NetworkStream stream = bezeroa.GetStream();
                    StreamReader irakurlea = new StreamReader(stream);
                    StreamWriter idazlea = new StreamWriter(stream);
                    idazlea.AutoFlush = true;

                    BezeroEgoera egoera = new BezeroEgoera();

                    string? lerroa = irakurlea.ReadLine();
                    while (lerroa != null)
                    {
                        Erantzuna erantzuna;
                        if (beteta)
                            erantzuna = Erantzuna.Errorea("BETETA", "Zerbitzaria beteta dago (gehienez 10 bezero).");
                        else
                            erantzuna = Prozesatu(lerroa, egoera);

                        idazlea.WriteLine(JsonSerializer.Serialize(erantzuna));

                        if (beteta) break; // soberan dagoen bezeroari abisatu eta deskonektatu egiten da

                        lerroa = irakurlea.ReadLine();
                    }
                }
            }
            catch (IOException)
            {
                // Bezeroa bat-batean deskonektatu da: ez da ezer gertatzen.
            }
            finally
            {
                int geratzen = Interlocked.Decrement(ref _bezeroKopurua);
                Console.WriteLine("Bezero bat deskonektatu da. Aktiboak: " + geratzen);
            }
        }

        // JSON lerroa eskakizun bihurtzen du eta erantzuna itzultzen du.
        // Zerbaitek huts egiten badu, bezeroak errorea jasotzen du eta haria ez da erortzen.
        private Erantzuna Prozesatu(string lerroa, BezeroEgoera egoera)
        {
            try
            {
                Eskakizuna? eskakizuna = JsonSerializer.Deserialize<Eskakizuna>(lerroa);
                if (eskakizuna == null)
                    return Erantzuna.Errorea("ERROREA", "Eskakizuna hutsik.");

                return Exekutatu(eskakizuna, egoera);
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                return Erantzuna.Errorea("KONTRAESTRIKZIOA", ex.Message);
            }
            catch (StockEzaException ex)
            {
                return Erantzuna.Errorea("STOCK", ex.Message);
            }
            catch (Exception ex)
            {
                return Erantzuna.Errorea("ERROREA", "Zerbitzariaren errorea: " + ex.Message);
            }
        }

        private static Erantzuna Ukatu()
        {
            return Erantzuna.Errorea("EZ_BAIMENIK", "Ez duzu horretarako baimenik.");
        }

        // Aginduaren arabera, Db-ko dagokion metodoari deitzen dio.
        private Erantzuna Exekutatu(Eskakizuna e, BezeroEgoera egoera)
        {
            JsonElement d = e.Datuak ?? default;

            string rola = egoera.Erabiltzailea?.Rola ?? "";
            bool admin = rola == "admin";
            bool biltegia = rola == "admin" || rola == "langilea";

            // ---------- Saiorik gabe erabil daitezkeenak ----------
            switch (e.Agindua)
            {
                case "Login":
                    {
                        string erabiltzailea = d.GetProperty("Erabiltzailea").GetString() ?? "";
                        string pasahitza = d.GetProperty("Pasahitza").GetString() ?? "";

                        Erabiltzailea? u = Db.Login(erabiltzailea, pasahitza);
                        if (u == null)
                            return Erantzuna.Errorea("LOGIN_OKERRA", "Erabiltzailea edo pasahitza okerra.");

                        egoera.Erabiltzailea = u;
                        return Erantzuna.Zuzena(u);
                    }

                case "Logout":
                    egoera.Erabiltzailea = null;
                    return Erantzuna.Zuzena();

                case "GetMotak":
                    return Erantzuna.Zuzena(Db.GetMotak());

                case "GetProduktuak":
                    return Erantzuna.Zuzena(Db.GetProduktuak(d.GetProperty("MotaId").GetInt32()));

                case "GetMahaiak":
                    return Erantzuna.Zuzena(Db.GetMahaiak());
            }

            // ---------- Hemendik aurrera saioa behar da ----------
            if (egoera.Erabiltzailea == null)
                return Erantzuna.Errorea("EZ_BAIMENIK", "Saioa hasi behar duzu.");

            switch (e.Agindua)
            {
                // Erreserbak: erabiltzaile guztiak
                case "GetErreserbak":
                    return Erantzuna.Zuzena(Db.GetErreserbak(
                        d.GetProperty("Data").GetString() ?? "",
                        d.GetProperty("Txanda").GetString() ?? ""));

                case "GordeErreserba":
                    Db.GordeErreserba(d.Deserialize<Erreserba>()!);
                    return Erantzuna.Zuzena();

                case "EguneratuErreserba":
                    Db.EguneratuErreserba(d.Deserialize<Erreserba>()!);
                    return Erantzuna.Zuzena();

                case "EzabatuErreserba":
                    Db.EzabatuErreserba(d.GetProperty("Id").GetInt32());
                    return Erantzuna.Zuzena();

                // Eskaera: erabiltzailea saiotik hartzen da, ez bezeroak bidalitakotik
                case "GordeEskaera":
                    {
                        JsonElement mahaiaDatua = d.GetProperty("MahaiId");
                        int? mahaiId = mahaiaDatua.ValueKind == JsonValueKind.Null ? null : mahaiaDatua.GetInt32();
                        List<EskaeraLerroa> lerroak = d.GetProperty("Lerroak").Deserialize<List<EskaeraLerroa>>()!;

                        int id = Db.GordeEskaera(egoera.Erabiltzailea.Id, mahaiId, lerroak);
                        return Erantzuna.Zuzena(id);
                    }

                // Biltegia: admin eta langilea (ezabatzea admin bakarrik)
                case "GetProduktuakBiltegia":
                    if (!biltegia) return Ukatu();
                    return Erantzuna.Zuzena(Db.GetProduktuakBiltegia());

                case "GordeProduktua":
                    if (!biltegia) return Ukatu();
                    Db.GordeProduktua(d.Deserialize<Produktua>()!);
                    return Erantzuna.Zuzena();

                case "EguneratuProduktua":
                    if (!biltegia) return Ukatu();
                    Db.EguneratuProduktua(d.Deserialize<Produktua>()!);
                    return Erantzuna.Zuzena();

                case "EzabatuProduktua":
                    if (!admin) return Ukatu();
                    Db.EzabatuProduktua(d.GetProperty("Id").GetInt32());
                    return Erantzuna.Zuzena();

                // Erabiltzaileak: admin bakarrik
                case "GetErabiltzaileak":
                    if (!admin) return Ukatu();
                    return Erantzuna.Zuzena(Db.GetErabiltzaileak());

                case "AdminKopurua":
                    if (!admin) return Ukatu();
                    return Erantzuna.Zuzena(Db.AdminKopurua());

                case "GordeErabiltzailea":
                    if (!admin) return Ukatu();
                    Db.GordeErabiltzailea(d.Deserialize<Erabiltzailea>()!);
                    return Erantzuna.Zuzena();

                case "EguneratuErabiltzailea":
                    if (!admin) return Ukatu();
                    Db.EguneratuErabiltzailea(d.Deserialize<Erabiltzailea>()!);
                    return Erantzuna.Zuzena();

                case "EzabatuErabiltzailea":
                    if (!admin) return Ukatu();
                    Db.EzabatuErabiltzailea(d.GetProperty("Id").GetInt32());
                    return Erantzuna.Zuzena();

                default:
                    return Erantzuna.Errorea("EZEZAGUNA", "Agindu ezezaguna: " + e.Agindua);
            }
        }
    }
}

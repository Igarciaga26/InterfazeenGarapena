using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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

    public static class Program
    {
        private const int Ataka = 5050;
        private const int GehienezBezeroak = 10;
        private static int aktiboak = 0;

        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Db.IrudiakEsleitu();
            X509Certificate2 ziurtagiria = ZiurtagiriaLortu();

            TcpListener entzulea = new TcpListener(IPAddress.Any, Ataka);
            entzulea.Start();
            Console.WriteLine("TPV zerbitzaria martxan, " + Ataka + " portuan (gehienez " + GehienezBezeroak + " bezero).");

            while (true)
            {
                TcpClient bezeroa = await entzulea.AcceptTcpClientAsync();
                _ = Task.Run(() => BezeroaKudeatu(bezeroa, ziurtagiria));
            }
        }

        // TLSrako ziurtagiri autosinatua sortzen du lehen aldian, eta fitxategian gordetzen du
        private static X509Certificate2 ZiurtagiriaLortu()
        {
            string bidea = Path.Combine(AppContext.BaseDirectory, "zerbitzaria.pfx");

            if (!File.Exists(bidea))
            {
                using RSA rsa = RSA.Create(2048);
                CertificateRequest ziurtagiriEskaera = new CertificateRequest(
                    "CN=TPV-Zerbitzaria", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                ziurtagiriEskaera.CertificateExtensions.Add(new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));
                ziurtagiriEskaera.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(
                    new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

                using X509Certificate2 sortua = ziurtagiriEskaera.CreateSelfSigned(
                    DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(5));

                File.WriteAllBytes(bidea, sortua.Export(X509ContentType.Pfx));
                Console.WriteLine("Ziurtagiri berria sortu da: " + bidea);
            }

            return X509CertificateLoader.LoadPkcs12FromFile(bidea, null);
        }

        // Bezero bakoitza bere ataza independentean
        private static async Task BezeroaKudeatu(TcpClient bezeroa, X509Certificate2 ziurtagiria)
        {
            string helbidea = bezeroa.Client.RemoteEndPoint?.ToString() ?? "?";
            int kopurua = Interlocked.Increment(ref aktiboak);

            try
            {
                using (bezeroa)
                using (SslStream ssl = new SslStream(bezeroa.GetStream(), false))
                {
                    await ssl.AuthenticateAsServerAsync(ziurtagiria);

                    using StreamReader irakurlea = new StreamReader(ssl, Encoding.UTF8);
                    using StreamWriter idazlea = new StreamWriter(ssl, new UTF8Encoding(false)) { AutoFlush = true };

                    if (kopurua > GehienezBezeroak)
                    {
                        Console.WriteLine("Baztertua (zerbitzaria beteta): " + helbidea);
                        await idazlea.WriteLineAsync(JsonSerializer.Serialize(
                            Erantzuna.Errorea("BETETA", "Zerbitzaria beteta dago (gehienez 10 bezero).")));
                        return;
                    }

                    Console.WriteLine("Konektatu da: " + helbidea + " (" + kopurua + " aktibo)");

                    BezeroEgoera egoera = new BezeroEgoera();
                    string? lerroa;

                    while ((lerroa = await irakurlea.ReadLineAsync()) != null)
                    {
                        Erantzuna erantzuna;
                        try
                        {
                            Eskakizuna eskakizuna = JsonSerializer.Deserialize<Eskakizuna>(lerroa)!;
                            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + helbidea + " -> " + eskakizuna.Agindua);
                            erantzuna = Prozesatu(eskakizuna, egoera);
                        }
                        catch (Exception ex)
                        {
                            erantzuna = Erantzuna.Errorea("ERROREA", "Eskakizun okerra: " + ex.Message);
                        }

                        await idazlea.WriteLineAsync(JsonSerializer.Serialize(erantzuna));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errorea " + helbidea + " bezeroarekin: " + ex.Message);
            }
            finally
            {
                Interlocked.Decrement(ref aktiboak);
                Console.WriteLine("Deskonektatu da: " + helbidea);
            }
        }

        private static Erantzuna Ukatu()
        {
            return Erantzuna.Errorea("EZ_BAIMENIK", "Ez duzu horretarako baimenik.");
        }

        private static Erantzuna Prozesatu(Eskakizuna e, BezeroEgoera egoera)
        {
            try
            {
                JsonElement d = e.Datuak ?? default;

                string rola = egoera.Erabiltzailea?.Rola ?? "";
                bool saioaDu = egoera.Erabiltzailea != null;
                bool admin = rola == "admin";
                bool biltegia = rola == "admin" || rola == "langilea";

                switch (e.Agindua)
                {
                    // ---------- Saiorik gabe erabil daitezkeenak ----------
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
                if (!saioaDu)
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

                            int id = Db.GordeEskaera(egoera.Erabiltzailea!.Id, mahaiId, lerroak);
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
                Console.WriteLine("Errorea: " + ex.Message);
                return Erantzuna.Errorea("ERROREA", ex.Message);
            }
        }
    }
}
using Erronka.IgorGarcia;
using Microsoft.Data.Sqlite;

namespace Erronka.Zerbitzaria
{
    // Stock nahikorik ez dagoenean jaurtitzen da
    public class StockEzaException : Exception
    {
        public StockEzaException(string mezua) : base(mezua) { }
    }

    public static class Db
    {
        private static readonly string ConnectionString =
            "Data Source=" + Path.Combine(AppContext.BaseDirectory, "tpv.db");

        // DBko idazketa guztiak blokeo bakar honekin egiten dira
        private static readonly object Idazketa = new object();

        private static SqliteConnection Ireki()
        {
            SqliteConnection konexioa = new SqliteConnection(ConnectionString);
            konexioa.Open();
            return konexioa;
        }

        // ================= LOGIN =================

        public static Erabiltzailea? Login(string erabiltzailea, string pasahitza)
        {
            using SqliteConnection konexioa = Ireki();
            using SqliteCommand cmd = konexioa.CreateCommand();
            cmd.CommandText = @"SELECT id, izena, rola
                                FROM erabiltzaileak
                                WHERE (emaila = $e OR izena = $e) AND pasahitza = $p";
            cmd.Parameters.AddWithValue("$e", erabiltzailea);
            cmd.Parameters.AddWithValue("$p", pasahitza);

            using SqliteDataReader irakurlea = cmd.ExecuteReader();
            if (!irakurlea.Read())
                return null;

            return new Erabiltzailea
            {
                Id = irakurlea.GetInt32(0),
                Izena = irakurlea.GetString(1),
                Rola = irakurlea.GetString(2)
            };
        }

        // ================= PRODUKTUAK =================

        public static List<ProduktuMota> GetMotak()
        {
            List<ProduktuMota> zerrenda = new List<ProduktuMota>();
            using SqliteConnection konexioa = Ireki();
            using SqliteCommand cmd = konexioa.CreateCommand();
            cmd.CommandText = "SELECT id, izena FROM produktu_motak ORDER BY id";

            using SqliteDataReader irakurlea = cmd.ExecuteReader();
            while (irakurlea.Read())
            {
                zerrenda.Add(new ProduktuMota
                {
                    Id = irakurlea.GetInt32(0),
                    Izena = irakurlea.GetString(1)
                });
            }
            return zerrenda;
        }

        // motaId = 0 bada, produktu guztiak itzultzen ditu.
        public static List<Produktua> GetProduktuak(int motaId)
        {
            List<Produktua> zerrenda = new List<Produktua>();
            using SqliteConnection konexioa = Ireki();
            using SqliteCommand cmd = konexioa.CreateCommand();
            cmd.CommandText = @"SELECT id, izena, prezioa, stock, mota_id, irudia
                                FROM produktuak
                                WHERE $m = 0 OR mota_id = $m
                                ORDER BY izena";
            cmd.Parameters.AddWithValue("$m", motaId);

            using SqliteDataReader irakurlea = cmd.ExecuteReader();
            while (irakurlea.Read())
            {
                zerrenda.Add(new Produktua
                {
                    Id = irakurlea.GetInt32(0),
                    Izena = irakurlea.GetString(1),
                    Prezioa = irakurlea.GetDouble(2),
                    Stock = irakurlea.GetInt32(3),
                    MotaId = irakurlea.GetInt32(4),
                    Irudia = irakurlea.IsDBNull(5) ? null : irakurlea.GetString(5)
                });
            }
            return zerrenda;
        }

        public static List<Produktua> GetProduktuakBiltegia()
        {
            List<Produktua> zerrenda = new List<Produktua>();
            using SqliteConnection konexioa = Ireki();
            using SqliteCommand cmd = konexioa.CreateCommand();
            cmd.CommandText = @"SELECT p.id, p.izena, p.prezioa, p.stock, p.mota_id, m.izena, p.irudia
                                FROM produktuak p
                                JOIN produktu_motak m ON m.id = p.mota_id
                                ORDER BY m.izena, p.izena";

            using SqliteDataReader irakurlea = cmd.ExecuteReader();
            while (irakurlea.Read())
            {
                zerrenda.Add(new Produktua
                {
                    Id = irakurlea.GetInt32(0),
                    Izena = irakurlea.GetString(1),
                    Prezioa = irakurlea.GetDouble(2),
                    Stock = irakurlea.GetInt32(3),
                    MotaId = irakurlea.GetInt32(4),
                    MotaIzena = irakurlea.GetString(5),
                    Irudia = irakurlea.IsDBNull(6) ? null : irakurlea.GetString(6)
                });
            }
            return zerrenda;
        }

        public static void GordeProduktua(Produktua p)
        {
            lock (Idazketa)
            {
                using SqliteConnection konexioa = Ireki();
                using SqliteCommand cmd = konexioa.CreateCommand();
                cmd.CommandText = @"INSERT INTO produktuak (izena, prezioa, stock, mota_id)
                                    VALUES ($i, $p, $s, $m)";
                cmd.Parameters.AddWithValue("$i", p.Izena);
                cmd.Parameters.AddWithValue("$p", p.Prezioa);
                cmd.Parameters.AddWithValue("$s", p.Stock);
                cmd.Parameters.AddWithValue("$m", p.MotaId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void EguneratuProduktua(Produktua p)
        {
            lock (Idazketa)
            {
                using SqliteConnection konexioa = Ireki();
                using SqliteCommand cmd = konexioa.CreateCommand();
                cmd.CommandText = @"UPDATE produktuak
                                    SET izena = $i, prezioa = $p, stock = $s, mota_id = $m
                                    WHERE id = $id";
                cmd.Parameters.AddWithValue("$i", p.Izena);
                cmd.Parameters.AddWithValue("$p", p.Prezioa);
                cmd.Parameters.AddWithValue("$s", p.Stock);
                cmd.Parameters.AddWithValue("$m", p.MotaId);
                cmd.Parameters.AddWithValue("$id", p.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void EzabatuProduktua(int id)
        {
            lock (Idazketa)
            {
                using SqliteConnection konexioa = Ireki();
                using SqliteCommand cmd = konexioa.CreateCommand();
                cmd.CommandText = "DELETE FROM produktuak WHERE id = $id";
                cmd.Parameters.AddWithValue("$id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // Produktuen irudiak esleitzen ditu (hutsik daudenean soilik). Zerbitzaria abiatzean deitzen da.
        public static void IrudiakEsleitu()
        {
            string[,] irudiak =
            {
                { "Ogia", "pan.jpg" },
                { "Entsalada mistoa", "ensalada.jpg" },
                { "Gazta-platera", "gazta.jpeg" },
                { "Kroketak", "croquetas.jpg" },
                { "Patata bravak", "patatasbravas.jpg" },
                { "Gernikako piperrak", "pimientos.jpeg" },
                { "Txuleta", "txuleta.jpg" },
                { "Oilasko errea", "pollo.png" },
                { "Txahal-hanburgesa", "burguer.jpeg" },
                { "Txerri-saiheskia", "costilla.jpeg" },
                { "Arkume-txuletak", "chuletascordero.jpg" },
                { "Solomoa plantxan", "lomo.jpeg" },
                { "Bakailao pil-pilean", "bacalao.jpg" },
                { "Legatza saltsa berdean", "legatza.jpeg" },
                { "Antxoa frijituak", "anchoas-fritas.jpg" },
                { "Izokina plantxan", "izokina.jpg" },
                { "Txipiroiak tintan", "txipiroiak.jpg" },
                { "Lupia erreta", "lupia.jpeg" },
                { "Ura", "agua.jpg" },
                { "Kafea", "cafe.jpeg" },
                { "Kola", "cola.jpg" },
                { "Garagardoa", "cerveza.jpeg" },
                { "Ardo beltza (kopa)", "vino.jpg" },
                { "Sagardoa", "sagardo.jpeg" },
                { "Gazta-tarta", "gaztatarta.jpg" },
                { "Flana", "flan.jpg" },
                { "Arroz-esnea", "arrozconleche.jpeg" },
                { "Brownie", "brownie.jpeg" },
                { "Izozkia", "helado.jpeg" },
                { "Goxua", "goxua.jpg" }
            };

            lock (Idazketa)
            {
                using SqliteConnection konexioa = Ireki();
                using SqliteTransaction tx = konexioa.BeginTransaction();

                for (int i = 0; i < irudiak.GetLength(0); i++)
                {
                    using SqliteCommand cmd = konexioa.CreateCommand();
                    cmd.Transaction = tx;
                    cmd.CommandText = "UPDATE produktuak SET irudia = $i WHERE izena = $n AND (irudia IS NULL OR irudia = '')";
                    cmd.Parameters.AddWithValue("$i", irudiak[i, 1]);
                    cmd.Parameters.AddWithValue("$n", irudiak[i, 0]);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
        }

        // ================= MAHAIAK ETA ERRESERBAK =================

        public static List<Mahaia> GetMahaiak()
        {
            List<Mahaia> zerrenda = new List<Mahaia>();
            using SqliteConnection konexioa = Ireki();
            using SqliteCommand cmd = konexioa.CreateCommand();
            cmd.CommandText = "SELECT id, zenbakia, edukiera FROM mahaiak ORDER BY zenbakia";

            using SqliteDataReader irakurlea = cmd.ExecuteReader();
            while (irakurlea.Read())
            {
                int zenbakia = irakurlea.GetInt32(1);
                zerrenda.Add(new Mahaia
                {
                    Id = irakurlea.GetInt32(0),
                    Zenbakia = zenbakia,
                    Edukiera = irakurlea.GetInt32(2),
                    Izena = zenbakia + ". Mahaia"
                });
            }
            return zerrenda;
        }

        public static List<Erreserba> GetErreserbak(string data, string txanda)
        {
            List<Erreserba> zerrenda = new List<Erreserba>();
            using SqliteConnection konexioa = Ireki();
            using SqliteCommand cmd = konexioa.CreateCommand();
            cmd.CommandText = @"SELECT id, mahai_id, data, txanda, ordua, bezeroa,
                                       telefonoa, pertsona_kopurua, oharrak
                                FROM erreserbak
                                WHERE data = $d AND txanda = $t";
            cmd.Parameters.AddWithValue("$d", data);
            cmd.Parameters.AddWithValue("$t", txanda);

            using SqliteDataReader irakurlea = cmd.ExecuteReader();
            while (irakurlea.Read())
            {
                zerrenda.Add(new Erreserba
                {
                    Id = irakurlea.GetInt32(0),
                    MahaiId = irakurlea.GetInt32(1),
                    Data = irakurlea.GetString(2),
                    Txanda = irakurlea.GetString(3),
                    Ordua = irakurlea.GetString(4),
                    Bezeroa = irakurlea.GetString(5),
                    Telefonoa = irakurlea.IsDBNull(6) ? "" : irakurlea.GetString(6),
                    PertsonaKopurua = irakurlea.GetInt32(7),
                    Oharrak = irakurlea.IsDBNull(8) ? "" : irakurlea.GetString(8)
                });
            }
            return zerrenda;
        }

        public static void GordeErreserba(Erreserba e)
        {
            lock (Idazketa)
            {
                using SqliteConnection konexioa = Ireki();
                using SqliteCommand cmd = konexioa.CreateCommand();
                cmd.CommandText = @"INSERT INTO erreserbak
                                        (mahai_id, data, txanda, ordua, bezeroa, telefonoa, pertsona_kopurua, oharrak)
                                    VALUES ($m, $d, $t, $o, $b, $tel, $p, $oh)";
                cmd.Parameters.AddWithValue("$m", e.MahaiId);
                cmd.Parameters.AddWithValue("$d", e.Data);
                cmd.Parameters.AddWithValue("$t", e.Txanda);
                cmd.Parameters.AddWithValue("$o", e.Ordua);
                cmd.Parameters.AddWithValue("$b", e.Bezeroa);
                cmd.Parameters.AddWithValue("$tel", e.Telefonoa);
                cmd.Parameters.AddWithValue("$p", e.PertsonaKopurua);
                cmd.Parameters.AddWithValue("$oh", e.Oharrak);
                cmd.ExecuteNonQuery();
            }
        }

        public static void EguneratuErreserba(Erreserba e)
        {
            lock (Idazketa)
            {
                using SqliteConnection konexioa = Ireki();
                using SqliteCommand cmd = konexioa.CreateCommand();
                cmd.CommandText = @"UPDATE erreserbak
                                    SET ordua = $o, bezeroa = $b, telefonoa = $tel,
                                        pertsona_kopurua = $p, oharrak = $oh
                                    WHERE id = $id";
                cmd.Parameters.AddWithValue("$o", e.Ordua);
                cmd.Parameters.AddWithValue("$b", e.Bezeroa);
                cmd.Parameters.AddWithValue("$tel", e.Telefonoa);
                cmd.Parameters.AddWithValue("$p", e.PertsonaKopurua);
                cmd.Parameters.AddWithValue("$oh", e.Oharrak);
                cmd.Parameters.AddWithValue("$id", e.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void EzabatuErreserba(int id)
        {
            lock (Idazketa)
            {
                using SqliteConnection konexioa = Ireki();
                using SqliteCommand cmd = konexioa.CreateCommand();
                cmd.CommandText = "DELETE FROM erreserbak WHERE id = $id";
                cmd.Parameters.AddWithValue("$id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ================= ESKAERAK =================

        // Eskaera gorde eta stock-a kendu. Eskaeraren id-a itzultzen du.
        // Stock nahikorik ez badago, StockEzaException jaurtitzen du eta ez da ezer gordetzen.
        public static int GordeEskaera(int erabiltzaileId, int? mahaiId, List<EskaeraLerroa> lista)
        {
            double guztira = lista.Sum(l => l.Prezioa * l.Kantitatea);

            lock (Idazketa)
            {
                using SqliteConnection konexioa = Ireki();
                using SqliteTransaction tx = konexioa.BeginTransaction();

                int eskaeraId;
                using (SqliteCommand cmd = konexioa.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"INSERT INTO eskaerak (erabiltzaile_id, mahai_id, guztira)
                                        VALUES ($u, $m, $g);
                                        SELECT last_insert_rowid();";
                    cmd.Parameters.AddWithValue("$u", erabiltzaileId);
                    cmd.Parameters.AddWithValue("$m", (object?)mahaiId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$g", guztira);
                    eskaeraId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                foreach (EskaeraLerroa lerroa in lista)
                {
                    using (SqliteCommand cmd = konexioa.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"INSERT INTO eskaera_lerroak (eskaera_id, produktu_id, kantitatea, prezioa)
                                            VALUES ($e, $p, $k, $pr)";
                        cmd.Parameters.AddWithValue("$e", eskaeraId);
                        cmd.Parameters.AddWithValue("$p", lerroa.ProduktuId);
                        cmd.Parameters.AddWithValue("$k", lerroa.Kantitatea);
                        cmd.Parameters.AddWithValue("$pr", lerroa.Prezioa);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqliteCommand cmd = konexioa.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"UPDATE produktuak SET stock = stock - $k
                                            WHERE id = $p AND stock >= $k";
                        cmd.Parameters.AddWithValue("$k", lerroa.Kantitatea);
                        cmd.Parameters.AddWithValue("$p", lerroa.ProduktuId);

                        if (cmd.ExecuteNonQuery() == 0)
                            throw new StockEzaException("Stock nahikorik ez: " + lerroa.Izena);
                    }
                }

                tx.Commit();
                return eskaeraId;
            }
        }

        // ================= ERABILTZAILEAK =================

        public static List<Erabiltzailea> GetErabiltzaileak()
        {
            List<Erabiltzailea> zerrenda = new List<Erabiltzailea>();
            using SqliteConnection konexioa = Ireki();
            using SqliteCommand cmd = konexioa.CreateCommand();
            cmd.CommandText = "SELECT id, izena, emaila, rola FROM erabiltzaileak ORDER BY izena";

            using SqliteDataReader irakurlea = cmd.ExecuteReader();
            while (irakurlea.Read())
            {
                zerrenda.Add(new Erabiltzailea
                {
                    Id = irakurlea.GetInt32(0),
                    Izena = irakurlea.GetString(1),
                    Emaila = irakurlea.GetString(2),
                    Rola = irakurlea.GetString(3)
                });
            }
            return zerrenda;
        }

        public static int AdminKopurua()
        {
            using SqliteConnection konexioa = Ireki();
            using SqliteCommand cmd = konexioa.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM erabiltzaileak WHERE rola = 'admin'";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static void GordeErabiltzailea(Erabiltzailea e)
        {
            lock (Idazketa)
            {
                using SqliteConnection konexioa = Ireki();
                using SqliteCommand cmd = konexioa.CreateCommand();
                cmd.CommandText = @"INSERT INTO erabiltzaileak (izena, emaila, pasahitza, rola)
                                    VALUES ($i, $e, $p, $r)";
                cmd.Parameters.AddWithValue("$i", e.Izena);
                cmd.Parameters.AddWithValue("$e", e.Emaila);
                cmd.Parameters.AddWithValue("$p", e.Pasahitza);
                cmd.Parameters.AddWithValue("$r", e.Rola);
                cmd.ExecuteNonQuery();
            }
        }

        // Pasahitza hutsik badago, lehengoa mantentzen da.
        public static void EguneratuErabiltzailea(Erabiltzailea e)
        {
            lock (Idazketa)
            {
                using SqliteConnection konexioa = Ireki();
                using SqliteCommand cmd = konexioa.CreateCommand();
                cmd.CommandText = @"UPDATE erabiltzaileak
                                    SET izena = $i, emaila = $e, rola = $r,
                                        pasahitza = CASE WHEN $p = '' THEN pasahitza ELSE $p END
                                    WHERE id = $id";
                cmd.Parameters.AddWithValue("$i", e.Izena);
                cmd.Parameters.AddWithValue("$e", e.Emaila);
                cmd.Parameters.AddWithValue("$r", e.Rola);
                cmd.Parameters.AddWithValue("$p", e.Pasahitza);
                cmd.Parameters.AddWithValue("$id", e.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void EzabatuErabiltzailea(int id)
        {
            lock (Idazketa)
            {
                using SqliteConnection konexioa = Ireki();
                using SqliteCommand cmd = konexioa.CreateCommand();
                cmd.CommandText = "DELETE FROM erabiltzaileak WHERE id = $id";
                cmd.Parameters.AddWithValue("$id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
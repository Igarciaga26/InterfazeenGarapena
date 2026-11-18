PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS erabiltzaileak (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    izena       TEXT NOT NULL,
    emaila      TEXT NOT NULL UNIQUE,
    pasahitza   TEXT NOT NULL,
    rola        TEXT NOT NULL DEFAULT 'langilea'
);

CREATE TABLE IF NOT EXISTS produktu_motak (
    id      INTEGER PRIMARY KEY AUTOINCREMENT,
    izena   TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS produktuak (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    izena       TEXT NOT NULL UNIQUE,
    prezioa     REAL NOT NULL CHECK (prezioa >= 0),
    stock       INTEGER NOT NULL DEFAULT 0,
    irudia      TEXT,
    mota_id     INTEGER NOT NULL,
    FOREIGN KEY (mota_id) REFERENCES produktu_motak(id)
);

CREATE TABLE IF NOT EXISTS mahaiak (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    zenbakia    INTEGER NOT NULL UNIQUE,
    edukiera    INTEGER NOT NULL CHECK (edukiera > 0)
);

CREATE TABLE IF NOT EXISTS erreserbak (
    id                INTEGER PRIMARY KEY AUTOINCREMENT,
    mahai_id          INTEGER NOT NULL,
    data              TEXT NOT NULL,
    txanda            TEXT NOT NULL CHECK (txanda IN ('Bazkaria','Afaria')),
    ordua             TEXT NOT NULL,
    bezeroa           TEXT NOT NULL,
    telefonoa         TEXT,
    pertsona_kopurua  INTEGER NOT NULL CHECK (pertsona_kopurua > 0),
    oharrak           TEXT,
    FOREIGN KEY (mahai_id) REFERENCES mahaiak(id),
    UNIQUE (mahai_id, data, txanda)
);

CREATE TABLE IF NOT EXISTS eskaerak (
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    data            TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    erabiltzaile_id INTEGER,
    mahai_id        INTEGER,
    guztira         REAL NOT NULL DEFAULT 0,
    FOREIGN KEY (erabiltzaile_id) REFERENCES erabiltzaileak(id),
    FOREIGN KEY (mahai_id) REFERENCES mahaiak(id)
);

CREATE TABLE IF NOT EXISTS eskaera_lerroak (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    eskaera_id  INTEGER NOT NULL,
    produktu_id INTEGER NOT NULL,
    kantitatea  INTEGER NOT NULL CHECK (kantitatea > 0),
    prezioa     REAL NOT NULL,
    FOREIGN KEY (eskaera_id) REFERENCES eskaerak(id) ON DELETE CASCADE,
    FOREIGN KEY (produktu_id) REFERENCES produktuak(id)
);

INSERT OR IGNORE INTO produktu_motak (izena) VALUES
    ('Hasierakoak'), ('Okelak'), ('Arrainak'), ('Edariak'), ('Postreak');

INSERT OR IGNORE INTO mahaiak (zenbakia, edukiera) VALUES
    (1,4),(2,4),(3,2),(4,6),(5,4),(6,2),(7,4),(8,6),(9,2);

INSERT OR IGNORE INTO erabiltzaileak (izena, emaila, pasahitza, rola) VALUES
    ('Admin', 'admin@tpv.eus', 'admin123', 'admin');

INSERT OR IGNORE INTO produktuak (izena, prezioa, stock, mota_id) VALUES
    ('Ogia',                    2.00,  50, (SELECT id FROM produktu_motak WHERE izena = 'Hasierakoak')),
    ('Entsalada mistoa',        7.50,  30, (SELECT id FROM produktu_motak WHERE izena = 'Hasierakoak')),
    ('Gazta-platera',           9.50,  25, (SELECT id FROM produktu_motak WHERE izena = 'Hasierakoak')),
    ('Kroketak',                6.50,  40, (SELECT id FROM produktu_motak WHERE izena = 'Hasierakoak')),
    ('Patata bravak',           5.50,  40, (SELECT id FROM produktu_motak WHERE izena = 'Hasierakoak')),
    ('Gernikako piperrak',      6.00,  30, (SELECT id FROM produktu_motak WHERE izena = 'Hasierakoak')),
    ('Txuleta',                18.50,  20, (SELECT id FROM produktu_motak WHERE izena = 'Okelak')),
    ('Oilasko errea',          11.00,  20, (SELECT id FROM produktu_motak WHERE izena = 'Okelak')),
    ('Txahal-hanburgesa',      10.50,  30, (SELECT id FROM produktu_motak WHERE izena = 'Okelak')),
    ('Txerri-saiheskia',       12.00,  20, (SELECT id FROM produktu_motak WHERE izena = 'Okelak')),
    ('Arkume-txuletak',        16.00,  15, (SELECT id FROM produktu_motak WHERE izena = 'Okelak')),
    ('Solomoa plantxan',       12.50,  20, (SELECT id FROM produktu_motak WHERE izena = 'Okelak')),
    ('Bakailao pil-pilean',    15.00,  15, (SELECT id FROM produktu_motak WHERE izena = 'Arrainak')),
    ('Legatza saltsa berdean', 14.50,  15, (SELECT id FROM produktu_motak WHERE izena = 'Arrainak')),
    ('Antxoa frijituak',        9.00,  25, (SELECT id FROM produktu_motak WHERE izena = 'Arrainak')),
    ('Izokina plantxan',       15.50,  15, (SELECT id FROM produktu_motak WHERE izena = 'Arrainak')),
    ('Txipiroiak tintan',      13.50,  15, (SELECT id FROM produktu_motak WHERE izena = 'Arrainak')),
    ('Lupia erreta',           17.00,  10, (SELECT id FROM produktu_motak WHERE izena = 'Arrainak')),
    ('Ura',                     1.50, 100, (SELECT id FROM produktu_motak WHERE izena = 'Edariak')),
    ('Kafea',                   1.40, 100, (SELECT id FROM produktu_motak WHERE izena = 'Edariak')),
    ('Kola',                    2.50,  80, (SELECT id FROM produktu_motak WHERE izena = 'Edariak')),
    ('Garagardoa',              2.80,  80, (SELECT id FROM produktu_motak WHERE izena = 'Edariak')),
    ('Ardo beltza (kopa)',      3.00,  60, (SELECT id FROM produktu_motak WHERE izena = 'Edariak')),
    ('Sagardoa',                3.50,  50, (SELECT id FROM produktu_motak WHERE izena = 'Edariak')),
    ('Gazta-tarta',             5.50,  20, (SELECT id FROM produktu_motak WHERE izena = 'Postreak')),
    ('Flana',                   4.00,  25, (SELECT id FROM produktu_motak WHERE izena = 'Postreak')),
    ('Arroz-esnea',             4.00,  25, (SELECT id FROM produktu_motak WHERE izena = 'Postreak')),
    ('Brownie',                 5.00,  20, (SELECT id FROM produktu_motak WHERE izena = 'Postreak')),
    ('Izozkia',                 3.50,  40, (SELECT id FROM produktu_motak WHERE izena = 'Postreak')),
    ('Goxua',                   4.50,  20, (SELECT id FROM produktu_motak WHERE izena = 'Postreak'));
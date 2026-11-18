# TPV Elkartea

Taberna edo elkarte baten eguneroko salmentak kudeatzeko aplikazioa (TPV). Honekin eskaerak hartu daitezke, mahaiak erreserbatu, biltegiko stocka kontrolatu eta erabiltzaileak kudeatu.

Aplikazioak bezero-zerbitzari egitura du:

- **Bezeroa** (`Erronka.IgorGarcia`): WPF aplikazioa, erabiltzaileak ikusten duena.
- **Zerbitzaria** (`ErronkaZerbitzaria`): kontsola aplikazioa. Datu-basearekin (`tpv.db`, SQLite) hitz egiten duen bakarra da, eta bezeroarekin TCP bidez komunikatzen da (5050 portua).

## Beharrezkoa

- Windows (WPF erabiltzen delako)
- Visual Studio, ".NET mahaigaineko garapena" osagaiarekin
- .NET SDK (proiektuak `net10.0` erabiltzen du)

## Aplikazioa martxan jartzea

1. Biltegia klonatu:

   ```
   git clone https://github.com/Igarciaga26/InterfazeenGarapena.git
   ```

2. Visual Studion soluzioa ireki (`.sln` fitxategia).

3. Egiaztatu `tpv.db` fitxategia **zerbitzariaren** proiektuan (`ErronkaZerbitzaria`) dagoela, eta bere propietateetan *Copiar en el directorio de salida* aukera *Copiar si es posterior* dela.

4. Menuan, *Compilar > Recompilar solución* aukeratu. Erroreik gabe amaitu behar du.

5. Bi proiektuak abiarazi, **zerbitzaria lehenengo** eta bezeroa gero. Horretarako, soluzioan eskuineko botoiarekin *Configurar proyectos de inicio* aukeratu eta bi proiektuak "Iniciar" ezarri, zerbitzaria goian dagoela.

6. Zerbitzariaren kontsolan mezu hau agertu behar da:

   ```
   TPV zerbitzaria martxan, 5050 portuan
   ```

7. Bezeroan saioa hasteko leihoa agertuko da. Sartu erabiltzaile baten datuekin (ikus hurrengo atala).

## Erabiltzaileak

| Erabiltzailea | Pasahitza | Rola |
| --- | --- | --- |
| `admin@tpv.eus` | _admin123_ | admin |

Beste erabiltzaile batzuk administratzaileak sor ditzake aplikazioaren *Erabiltzaileak* atalean.

SQLite datu-baseak ez du erabiltzailerik edo pasahitzik; fitxategi lokal bat da.

## Ohiko arazoak

- **Zerbitzariak `tpv.db` ez duela aurkitzen dio:** fitxategia zerbitzariaren proiektuan egon behar da. Zerbitzariaren `bin\Debug\net10.0` karpetan datu-base huts bat sortu bada, ezabatu eta berriro konpilatu.
- **`no such table` errorea:** zerbitzaria datu-base huts bat erabiltzen ari da; aurreko puntuaren konponbide bera.
- **Bezeroak "Errorea zerbitzariarekin komunikatzean" erakusten du:** zerbitzaria ez dago abiarazita edo 5050 portua blokeatuta dago.
- **Zerbitzariak `Protokoloa.cs` ez du aurkitzen:** zerbitzariaren `.csproj` fitxategiko `Compile Include` bidea zuzena izan behar da (bi karpetak elkarren ondoan egon behar dute).

## Dokumentazioa

- **Gida teknikoa:** sistemaren arkitektura, datu-basea eta komunikazio-protokoloa.
- **Erabiltzaile eta administratzaile gida:** aplikazioa nola erabili.

## Egilea

Igor García

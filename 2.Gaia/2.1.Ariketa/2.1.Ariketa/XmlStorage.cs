using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace _2._1.Ariketa
{
    public static class XmlStorage
    {
        private static string Bidea =>
            Path.Combine(AppContext.BaseDirectory, "Data", "atazak.xml");

        public static List<Ataza> Kargatu()
        {
            if (!File.Exists(Bidea))
                return new List<Ataza>();

            var doc = XDocument.Load(Bidea);
            return doc.Root!.Elements("Ataza").Select(x => new Ataza
            {
                Id = (int)x.Attribute("id")!,
                Titulua = ((string?)x.Element("Titulua"))?.Trim() ?? "",
                Lehentasuna = ((string?)x.Element("Lehentasuna"))?.Trim() ?? "Ertaina",
                AzkenEguna = DateTime.Parse(
                    ((string?)x.Element("AzkenEguna"))!.Trim(),
                    CultureInfo.InvariantCulture),
                Egina = ((string?)x.Element("Egoera"))?.Trim() == "Egina"
            }).ToList();
        }

        public static void Gorde(IEnumerable<Ataza> atazak)
        {
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("Atazak",
                    atazak.Select(a => new XElement("Ataza",
                        new XAttribute("id", a.Id),
                        new XElement("Titulua", a.Titulua),
                        new XElement("Lehentasuna", a.Lehentasuna),
                        new XElement("AzkenEguna", a.AzkenEguna.ToString("yyyy-MM-dd")),
                        new XElement("Egoera", a.Egina ? "Egina" : "Egin gabe")))));

            Directory.CreateDirectory(Path.GetDirectoryName(Bidea)!);
            doc.Save(Bidea);
        }
    }
}
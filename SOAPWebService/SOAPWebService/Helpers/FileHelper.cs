using CsvHelper;
using CsvHelper.Configuration;
using SOAPWebService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SOAPWebService.Helpers
{
    public static class FileHelper
    {
        private static readonly string XmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Wines.xml");
        private static readonly string CsvFilePath = @"C:\Users\38598\Documents\Faks\5. semestar 2003-2004\Interoperabilnost\Projekt\wine_data.csv"; 

        public static List<Wine> LoadWinesFromCsv()
        {
            var config = new CsvConfiguration { CultureInfo = CultureInfo.InvariantCulture };
            using (var reader = new StreamReader(CsvFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                return new List<Wine>(csv.GetRecords<Wine>());
            }
        }

        public static void SaveWinesToXml(List<Wine> wines)
        {
            var serializer = new XmlSerializer(typeof(List<Wine>));
            using (var writer = new StreamWriter(XmlFilePath))
            {
                serializer.Serialize(writer, wines);
            }
        }

        public static List<Wine> LoadWinesFromXml()
        {
            if (!File.Exists(XmlFilePath))
            {
                return new List<Wine>();
            }

            var serializer = new XmlSerializer(typeof(List<Wine>));
            using (var reader = new StreamReader(XmlFilePath))
            {
                return (List<Wine>)serializer.Deserialize(reader);
            }
        }

        public static void SyncCsvToXml()
        {
            var wines = LoadWinesFromCsv();
            SaveWinesToXml(wines);
        }
    }
}

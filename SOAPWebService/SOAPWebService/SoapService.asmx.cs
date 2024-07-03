using SOAPWebService.Helpers;
using SOAPWebService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Services;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

namespace SOAPWebService
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class SoapService : WebService
    {
    

        [WebMethod]
        public string GetWine(string name)
        {
            var wines = FileHelper.LoadWinesFromXml();
            var matchingWines = wines.Where(w => w.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!matchingWines.Any())
            {
                return "Wine not found";
            }

            var xmlSerializer = new XmlSerializer(typeof(List<Wine>));
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, matchingWines);
                return stringWriter.ToString();
            }
        }

        [WebMethod]
        public string AddWine(Wine wine)
        {
            var wines = FileHelper.LoadWinesFromXml();
            wines.Add(wine);
            FileHelper.SaveWinesToXml(wines);
            return "Wine successfully added";
        }

        [WebMethod]
        public void SyncCsvToXml()
        {
            FileHelper.SyncCsvToXml();
        }
    }
}
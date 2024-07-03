using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using WineClientApplication.Models;
using SoapServiceReference;

using System.IO;

namespace WineClientApplication.Controllers
{
    public class SoapController : Controller
    {
        private readonly SoapServiceSoapClient _soapClient;

        public SoapController()
        {
            _soapClient = new SoapServiceSoapClient(SoapServiceSoapClient.EndpointConfiguration.SoapServiceSoap);
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

       [HttpPost]
    public async Task<IActionResult> Search(string wineName)
    {
        var response = await _soapClient.GetWineAsync(wineName);
        var xml = response.Body.GetWineResult;

        if (string.IsNullOrEmpty(xml))
        {
            ViewBag.Message = "Wine not found.";
            return View("Result", null);
        }

        var wineResult = Deserialize(xml);
        if (wineResult == null || !wineResult.Any())
        {
            ViewBag.Message = "No such wine found.";
            return View("Result", null);
        }

        return View("Result", wineResult);
    }

        private List<WineClientApplication.Models.Wine> Deserialize(string xml)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<WineClientApplication.Models.Wine>));
                using StringReader reader = new StringReader(xml);
                return (List<WineClientApplication.Models.Wine>?)serializer.Deserialize(reader) ?? new List<WineClientApplication.Models.Wine>();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return new List<WineClientApplication.Models.Wine>();
            }
        }


        [HttpGet]
        public IActionResult AddWine()
        {
            return View();
        }

       
        [HttpPost]
        public async Task<IActionResult> AddWine(WineClientApplication.Models.Wine wine)
        {
            // Step 1: Serialize the wine object to XML
            var xmlSerializer = new XmlSerializer(typeof(WineClientApplication.Models.Wine));
            var xmlString = new StringWriter();

            using (var writer = XmlWriter.Create(xmlString, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
            {
                xmlSerializer.Serialize(writer, wine);
            }

            string xmlContent = xmlString.ToString();

            // Step 2: Deserialize the XML string to a SoapServiceReference.Wine object
            var soapXmlSerializer = new XmlSerializer(typeof(SoapServiceReference.Wine));
            SoapServiceReference.Wine soapWine;

            using (var reader = new StringReader(xmlContent))
            {
                soapWine = (SoapServiceReference.Wine)soapXmlSerializer.Deserialize(reader);
            }

            // Step 3: Pass the SoapServiceReference.Wine object to the AddWineAsync method
            var response = await _soapClient.AddWineAsync(soapWine);
            if (response.Body.AddWineResult == "Wine successfully added")
            {
                ViewBag.Message = "Wine successfully added!";
            }
            else
            {
                ViewBag.Message = $"Error: {response.Body.AddWineResult}";
            }

            return View();
        }
    }
    }
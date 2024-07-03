using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CsvHelper;
using System.Globalization;
using XSD.Models;

namespace XSD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WineController : ControllerBase
    {
        private static List<Wine> wines = new List<Wine>();

        static WineController()
        {
            LoadWinesFromCSV(@"C:\Users\38598\Documents\Faks\5. semestar 2003-2004\Interoperabilnost\Projekt\wine_data.csv");
        }

        [HttpGet]
        public IActionResult GetWines()
        {
            return Ok(wines);
        }

        [HttpPost]
        [Consumes("application/xml")]
        public IActionResult Post([FromBody] Wine wine)
        {
            var validationErrors = new List<string>();

            try
            {
                var xsd = Path.Combine(Directory.GetCurrentDirectory(), "XSD", "wine.xsd");
                var xmlSchemaSet = new XmlSchemaSet();
                xmlSchemaSet.Add("", xsd);

                var xmlSerializer = new XmlSerializer(typeof(Wine));
                var xmlString = new StringBuilder();

                using (var writer = new StringWriter(xmlString))
                {
                    xmlSerializer.Serialize(writer, wine);
                }

                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString.ToString());

                var settings = new XmlReaderSettings();
                settings.Schemas.Add(xmlSchemaSet);
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationEventHandler += (o, e) =>
                {
                    validationErrors.Add($"Line {e.Exception.LineNumber}, Position {e.Exception.LinePosition}: {e.Message}");
                };

                using (var reader = XmlReader.Create(new StringReader(xmlDocument.OuterXml), settings))
                {
                    while (reader.Read()) { }
                }

                if (validationErrors.Count > 0)
                {
                    return BadRequest(new { Message = "XML is not valid.", Errors = validationErrors });
                }

                wines.Add(wine);
                // Ispis vina koje je dodano
                Console.WriteLine($"Added wine: {wine}");

                return Ok("Wine successfully added.");
            }
            catch (XmlSchemaValidationException ex)
            {
                validationErrors.Add($"Error: {ex.Message}");
                return BadRequest(new { Message = "XML is not valid.", Errors = validationErrors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private static void LoadWinesFromCSV(string csvFilePath)
        {
            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                wines = csv.GetRecords<Wine>().ToList();

                // Ispis svih vina koja su učitana iz CSV datoteke
                foreach (var wine in wines)
                {
                    Console.WriteLine(wine);
                }
            }
        }
    }
}
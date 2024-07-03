using Commons.Xml.Relaxng;
using Microsoft.AspNetCore.Mvc;
using RNG.Models;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using CsvHelper;

namespace RNG.Controllers
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
                var rngPath = Path.Combine(Directory.GetCurrentDirectory(), "RNGSchema", "wine.rng");
                var xmlSerializer = new XmlSerializer(typeof(Wine));
                var xmlString = new StringBuilder();

                var settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                };

                using (var writer = XmlWriter.Create(new StringWriterWithEncoding(xmlString, Encoding.UTF8), settings))
                {
                    xmlSerializer.Serialize(writer, wine);
                }

                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString.ToString());

                using (var rngReader = XmlReader.Create(rngPath))
                {
                    using (var xmlReader = XmlReader.Create(new StringReader(xmlDocument.OuterXml)))
                    {
                        var validatingReader = new RelaxngValidatingReader(xmlReader, rngReader);

                        try
                        {
                            while (validatingReader.Read()) { }
                        }
                        catch (RelaxngException e)
                        {
                            validationErrors.Add(e.Message);
                        }
                    }
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

    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;
        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding) : base(sb)
        {
            this.encoding = encoding;
        }
        public override Encoding Encoding => encoding;
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using WineClientApplication.Models;
using System.Text.Json;


namespace WineClientApplication.Controllers
{
    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;
        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding) : base(sb)
        {
            this.encoding = encoding;
        }
        public override Encoding Encoding => encoding;
    }

    [ApiController]
    [Route("[controller]/[action]")]
    public class XsdController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public XsdController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult AddWine()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetWines()
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5200/");
            var response = await client.GetAsync("api/wine");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var wines = System.Text.Json.JsonSerializer.Deserialize<List<Wine>>(jsonString, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return View(wines);
            }

            ViewBag.Message = "Failed to fetch wines.";
            return View(new List<Wine>());
        }

        [HttpPost]
        public async Task<IActionResult> AddWine([FromForm] Wine wine)
        {
            if (wine == null)
            {
                ViewBag.Message = "Please provide wine data.";
                return View();
            }

            try
            {
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

                string xmlContent = xmlString.ToString();

                var client = _clientFactory.CreateClient();
                client.BaseAddress = new Uri("http://localhost:5200/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                using (var content = new StringContent(xmlContent, Encoding.UTF8, "application/xml"))
                {
                    var response = await client.PostAsync("api/wine", content);

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Wine successfully added!";
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        ViewBag.Message = $"Error: {error}";
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Message = $"Request error: {ex.Message}";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Internal Server Error: {ex.Message}";
            }

            return View();
        }
    }
}
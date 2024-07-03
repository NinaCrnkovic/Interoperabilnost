using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using WineClientApplication.Models;

namespace WineClientApplication.Controllers
{
    

    [ApiController]
    [Route("[controller]/[action]")]
    public class RngController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public RngController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult RngAddWine()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RngAddWine([FromForm] Wine wine)
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

        [HttpGet]
        public async Task<IActionResult> RngGetWines()
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
    }
}
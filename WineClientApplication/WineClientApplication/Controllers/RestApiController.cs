using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using WineClientApplication.Models;

namespace WineClientApplication.Controllers
{
    public class RestApiController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _url = "http://localhost:5200/api/Wine"; 

        public RestApiController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void AddBearerToken()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

      

        public async Task<IActionResult> RestIndex()
        {
            AddBearerToken();
            try
            {
                var response = await _httpClient.GetAsync(_url);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var wines = JsonSerializer.Deserialize<IEnumerable<Wine>>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(wines);
                }
                return View();
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error connecting to the server.");
                return View();
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Wine wine)
        {
            AddBearerToken();
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(wine), System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_url, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View(wine);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error connecting to the server.");
                return View(wine);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            AddBearerToken();
            try
            {
                var response = await _httpClient.GetAsync($"{_url}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var wine = JsonSerializer.Deserialize<Wine>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(wine);
                }
                return NotFound();
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error connecting to the server.");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Wine wine)
        {
            AddBearerToken();
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(wine), System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_url}/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View(wine);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error connecting to the server.");
                return View(wine);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            AddBearerToken();
            try
            {
                var response = await _httpClient.GetAsync($"{_url}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var wine = JsonSerializer.Deserialize<Wine>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(wine);
                }
                return NotFound();
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error connecting to the server.");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            AddBearerToken();
            try
            {
                var response = await _httpClient.DeleteAsync($"{_url}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View();
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error connecting to the server.");
                return View();
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Proyecto.Models;

namespace Proyecto.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://159.223.123.38:8000/") }; // Base URL de la API Flask
        }
        public async Task<ActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/listanegocios");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var negocios = JsonConvert.DeserializeObject<List<Negocio>>(data);


                return View(negocios);
            }
            else
            {
                TempData["Error"] = "Error al obtener los negocios";
                return View(new List<Negocio>());
            }
        }
    }

}
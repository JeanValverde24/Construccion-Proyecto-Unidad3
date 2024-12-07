using Newtonsoft.Json;
using Proyecto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Proyecto.Controllers
{
    public class HotelController : Controller
    {
        // GET: Hotel
        public ActionResult Index()
        {
            return View();
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        [HttpPost]
        public async Task<ActionResult> AgregarCuarto(int negocioId, ServicioHotel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.NegocioId = negocioId; // Reasigna para evitar pérdida en caso de error
                return View(model);
            }

            System.Diagnostics.Debug.WriteLine($"ID del negocio a enviar: {negocioId}");

            // Crear JSON con los datos a enviar, asegurando que los nombres coincidan con los esperados en la API
            var requestData = new
            {
                negocioId = negocioId,  // Asegúrate de usar "negocioId" y no "negocio_id"
                cantidadPersonas = model.CantidadPersonas,
                wifi = model.Wifi,
                aguaCaliente = model.AguaCaliente,
                roomService = model.RoomService,
                cochera = model.Cochera,
                cable = model.Cable,
                desayunoIncluido = model.DesayunoIncluido,
                precio = model.Precio
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var apiUrl = "http://159.223.123.38:8000/api/hoteles/agregar_cuarto";
            var response = await _httpClient.PostAsync(apiUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Servicios", "Business");
            }
            else
            {
                ViewBag.ErrorMessage = "Error al añadir el cuarto. Por favor, inténtelo de nuevo.";
                return View(model);
            }
        }

    }
}
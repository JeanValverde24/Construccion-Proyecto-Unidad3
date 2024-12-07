using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;
namespace Proyecto.Controllers
{
    public class RestauranteController : Controller
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        [HttpPost]
        public async Task<ActionResult> AgregarPlato(int negocioId, ServicioRestaurante model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var requestData = new
            {
                negocioId = negocioId,
                nombrePlato = model.NombrePlato,
                tipoPlato = model.TipoPlato,
                precio = model.Precio
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var apiUrl = "http://159.223.123.38:8000/api/restaurantes/agregar_plato";
            var response = await _httpClient.PostAsync(apiUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Confirmacion", "Restaurante");
            }
            else
            {
                ViewBag.ErrorMessage = "Error al añadir el plato. Por favor, inténtelo de nuevo.";
                return View(model);
            }
        }
    }

}
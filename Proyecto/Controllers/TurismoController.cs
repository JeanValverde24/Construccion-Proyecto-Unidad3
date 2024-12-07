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
    public class TurismoController : Controller
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        [HttpPost]
        public async Task<ActionResult> AgregarLugarTuristico(int negocioId, ServicioTurismo model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var requestData = new
            {
                negocioId = negocioId,
                nombreLugar = model.NombreLugar,
                descripcion = model.Descripcion,
                precio = model.Precio,
                provincia = model.Provincia
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var apiUrl = "http://159.223.123.38:8000/api/turismo/agregar_lugar";
            var response = await _httpClient.PostAsync(apiUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Confirmacion", "Turismo");
            }
            else
            {
                ViewBag.ErrorMessage = "Error al añadir el lugar turístico. Por favor, inténtelo de nuevo.";
                return View(model);
            }
        }
    }

}
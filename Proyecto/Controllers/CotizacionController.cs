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
    public class CotizacionController : Controller
    {
        // GET: Cotizacion
        private readonly HttpClient _httpClient;

        
        public ActionResult CotizacionResult()
        {
            return View();
        }

        public CotizacionController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://159.223.123.38:8000/api/"); // Cambia la URL base si es necesario
        }
        [HttpPost]
        public async Task<ActionResult> CustomQuote(
            int dias_viaje,
            decimal presupuesto_usuario,
            string motivo_viaje,
            int cantidad_personas,
            string ubicacion_usuario,
            string fecha_inicio,
            string fecha_fin)
        {
            var requestData = new
            {
                dias_viaje,
                presupuesto_max = presupuesto_usuario,
                motivo_viaje,
                cantidad_personas,
                ubicacion_usuario,
                fecha_inicio,
                fecha_fin
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("generar_cotizaciones", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // DEBUG: Imprimir el JSON recibido
                    System.Diagnostics.Debug.WriteLine("Respuesta JSON del backend:");
                    System.Diagnostics.Debug.WriteLine(jsonResponse);

                    // Verificar si la respuesta es un error
                    if (jsonResponse.Contains("\"error\""))
                    {
                        var errorObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
                        if (errorObject != null && errorObject.ContainsKey("error"))
                        {
                            ViewBag.ErrorMessage = $"Error recibido del backend: {errorObject["error"]}";
                            return View("CustomQuote");
                        }
                    }

                    // Deserializar la respuesta como lista de cotizaciones
                    var cotizaciones = JsonConvert.DeserializeObject<List<CotizacionResponse>>(jsonResponse);

                    return View("CotizacionResult", cotizaciones);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    // DEBUG: Imprimir el contenido de error recibido
                    System.Diagnostics.Debug.WriteLine("Error del backend:");
                    System.Diagnostics.Debug.WriteLine(errorContent);

                    ViewBag.ErrorMessage = $"Error al obtener la cotización: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                // DEBUG: Imprimir la excepción capturada
                System.Diagnostics.Debug.WriteLine("Excepción capturada:");
                System.Diagnostics.Debug.WriteLine(ex.Message);

                ViewBag.ErrorMessage = $"Error del servidor: {ex.Message}";
            }

            return View("CustomQuote");
        }








    }
}
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
    public class PerfilController : Controller
    {
        


        // PerfilController.cs

        // Acción para redirigir a CargarInformacionDelNegocio con el negocio_id
        public ActionResult DejarFeedback(int negocio_id)
        {
            return RedirectToAction("CargarInformacionDelNegocio", new { negocioId = negocio_id });
        }

        // Acción para cargar la información del negocio desde la API y enviar los datos a la vista DejarFeedback
        public async Task<ActionResult> CargarInformacionDelNegocio(int negocioId)
        {
            ViewBag.NegocioId = negocioId;

            // Llamada a la API
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://159.223.123.38:8000/");
                var response = await client.GetAsync($"api/obtener_negocio/{negocioId}");
                System.Diagnostics.Debug.WriteLine($"Estado de respuesta de la API: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var negocio = await response.Content.ReadAsAsync<Negocio>();
                    return View("DejarFeedback", negocio); // Envía a la vista DejarFeedback con los datos del negocio
                }
            }

            return HttpNotFound("Negocio no encontrado.");
        }



        [HttpPost]
        public async Task<ActionResult> EnviarFeedback(int negocio_id, string comentario, int estrellas)
        {
            var feedbackData = new
            {
                negocio_id = negocio_id,
                usuario_id = Session["IdUsuario"],
                comentario = comentario,
                calificacion = estrellas
            };
            System.Diagnostics.Debug.WriteLine($"Llamada a la API iniciada con correo: {feedbackData.negocio_id}");


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://159.223.123.38:8000/");
                var response = await client.PostAsJsonAsync("api/enviar_feedback", feedbackData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Feedback enviado correctamente";
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Hubo un error al enviar el feedback. Inténtalo nuevamente.";
                    return RedirectToAction("DejarFeedback", new { negocioId = negocio_id });
                }
            }
        }

        public async Task<ActionResult> Historial()
        {
            // Intenta convertir el valor de la sesión a int
            if (Session["IdUsuario"] == null || !int.TryParse(Session["IdUsuario"].ToString(), out int usuarioId))
            {
                // Si la conversión falla, redirige al usuario a la página de inicio de sesión
                return RedirectToAction("Index", "Login");
            }

            System.Diagnostics.Debug.WriteLine($"Valor de IdUsuario en la sesión: {usuarioId}");

            using (var client = new HttpClient())
            {
                string url = $"http://159.223.123.38:8000/api/feedbacks/{usuarioId}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    // Imprime el código de estado y el contenido de error para depurar
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error en la solicitud: {response.StatusCode} - {errorMessage}");
                    return new HttpStatusCodeResult(response.StatusCode, "Error al obtener los feedbacks.");
                }

                var responseData = await response.Content.ReadAsStringAsync();

                // Imprime el JSON recibido en la salida de depuración
                System.Diagnostics.Debug.WriteLine("JSON recibido:");
                System.Diagnostics.Debug.WriteLine(responseData);

                var feedbacks = JsonConvert.DeserializeObject<List<Feedback>>(responseData);
                return View(feedbacks);
            }
        }

        public async Task<ActionResult> HistorialFeedBackNegocio()
        {
            if (Session["IdUsuario"] == null || !int.TryParse(Session["IdUsuario"].ToString(), out int usuarioId))
            {
                return RedirectToAction("Index", "Login");
            }

            System.Diagnostics.Debug.WriteLine($"Valor de IdUsuario en la sesión: {usuarioId}");

            using (var client = new HttpClient())
            {
                string url = $"http://159.223.123.38:8000/api/negocios_feedbacks?user_id={usuarioId}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error en la solicitud: {response.StatusCode} - {errorMessage}");
                    return new HttpStatusCodeResult(response.StatusCode, "Error al obtener los negocios y feedbacks.");
                }

                var responseData = await response.Content.ReadAsStringAsync();

                // Imprime el JSON recibido en la consola para depuración
                System.Diagnostics.Debug.WriteLine("JSON recibido: " + responseData);

                var negocios = JsonConvert.DeserializeObject<List<NegocioConFeedback>>(responseData);
                return View(negocios);
            }
        }




    }

}
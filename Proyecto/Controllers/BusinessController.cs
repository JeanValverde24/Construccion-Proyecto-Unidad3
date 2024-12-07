using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto.Models;
using System.Net.Http.Headers; // Para trabajar con headers si es necesario
using System.Net.Http.Formatting; // Para ReadAsAsync

namespace Proyecto.Controllers
{
    public class BusinessController : Controller
    {
        private readonly HttpClient _httpClient;

        public BusinessController()
        {
            _httpClient = new HttpClient();
        }

        // GET: AñadirServicio
        public System.Web.Mvc.ActionResult AñadirServicio_Hotel(int negocioId)
        {
            ViewBag.NegocioId = negocioId; // Asigna el negocioId a ViewBag para pasarlo a la vista

            return View();
        }
        public async Task<System.Web.Mvc.ActionResult> Servicios()
        {
            var userId = Session["IdUsuario"];
            System.Diagnostics.Debug.WriteLine($"Valor de IdUsuario en la sesión: {Session["IdUsuario"]}");
            if (userId == null)
            {
                return RedirectToAction("Index", "Account");
            }

            var url = $"http://159.223.123.38:8000//api/negocios?user_id={userId}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var negocios = JsonConvert.DeserializeObject<List<Negocio>>(json);
                return View(negocios);
            }
            else
            {
                ViewBag.ErrorMessage = "No se pudieron cargar los negocios.";
                return View(new List<Negocio>());
            }
        }
        public System.Web.Mvc.ActionResult AñadirServicio_Restaurante(int negocioId)
        {
            ViewBag.NegocioId = negocioId; // Asigna el negocioId a ViewBag para pasarlo a la vista

            return View();
        }

        public async Task<System.Web.Mvc.ActionResult> CargarInformacionDelNegocio(int negocioId)
        {
            ViewBag.NegocioId = negocioId;

            // Llamada a la API
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://159.223.123.38:8000/"); // Cambia según tu configuración
            var response = await client.GetAsync($"api/obtener_negocio/{negocioId}");
            System.Diagnostics.Debug.WriteLine($"Estado de respuesta de la API: {response.StatusCode}");


            if (response.IsSuccessStatusCode)
            {
                var negocio = await response.Content.ReadAsAsync<Negocio>();
                return View(negocio);
            }

            return HttpNotFound("Negocio no encontrado.");
        }
        public async Task<System.Web.Mvc.ActionResult> CargarInformacionDelNegocioUsuario(int negocioId)
        {
            ViewBag.NegocioId = negocioId;

            // Incrementar visitas mediante la API
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://159.223.123.38:8000/"); // Cambia según tu configuración
                var payload = new { negocioId = negocioId };
                var response = await client.PostAsJsonAsync("api/incrementar_visitas", payload);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Error incrementando visitas: {response.StatusCode}");
                }
            }

            // Llamada a la API para obtener información del negocio
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://159.223.123.38:8000/");
                var response = await client.GetAsync($"api/obtener_negocio/{negocioId}");

                System.Diagnostics.Debug.WriteLine($"Estado de respuesta de la API: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var negocio = await response.Content.ReadAsAsync<Negocio>();
                    System.Diagnostics.Debug.WriteLine("Datos enviados a la vista:");
                    System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(negocio, Newtonsoft.Json.Formatting.Indented));

                    return View(negocio);
                }
            }

            return HttpNotFound("Negocio no encontrado.");
        }

        [System.Web.Mvc.HttpPost]
        public async Task<System.Web.Mvc.ActionResult> ActualizarNegocio(Negocio negocio)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://159.223.123.38:8000/");

            if (ModelState.IsValid)
            {
                var payload = new
                {
                    TbNgcNombre = negocio.TbNgcNombre,
                    TipoNegocio = negocio.TipoNegocio,
                    ProvinciaId = negocio.TbNgcProvincia  // El valor correcto de la provincia
                };

                var serializedPayload = JsonConvert.SerializeObject(payload);
                System.Diagnostics.Debug.WriteLine($"Payload enviado: {serializedPayload}");

                var response = await client.PutAsJsonAsync($"api/actualizarnegocio/{negocio.negocio_id}", payload);

                System.Diagnostics.Debug.WriteLine($"Estado de respuesta de la API: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Servicios");
                }

                ModelState.AddModelError("", "No se pudo actualizar el negocio.");
            }
            else
            {
                ModelState.AddModelError("", "Por favor, complete todos los campos requeridos.");
            }

            return View("CargarInformacionDelNegocio", negocio);
        }





        public System.Web.Mvc.ActionResult AñadirServicio_Turismo(int negocioId)
        {
            ViewBag.NegocioId = negocioId; // Asigna el negocioId a ViewBag para pasarlo a la vista

            return View();
        }
        public System.Web.Mvc.ActionResult Index_AñadirServicio(int negocioId)
        {
            // Guardar el negocioId en Session para su uso en toda la sesión
            Session["NegocioId"] = negocioId;

            return View();
        }
        public System.Web.Mvc.ActionResult Index_EditarServicios(int negocioId)
        {
            // Guardar el negocioId en Session para su uso en toda la sesión
            Session["NegocioId"] = negocioId;

            return View();
        }
        // GET: Dashboard con filtro
        public System.Web.Mvc.ActionResult Dashboard(string filtro = "")
        {
            // Simulación de datos que podrías obtener de la base de datos
            var servicios = new List<Servicio>
            {
                new Servicio
                {
                    Id = 1,
                    Nombre = "Hotel Pacific Suites",
                    Precio = 300.00,
                    NumeroHabitaciones = 45,
                    ServiciosIncluidos = "WiFi, Piscina, Desayuno"
                },
                new Servicio
                {
                    Id = 2,
                    Nombre = "Restaurante El Sabroso",
                    TipoComida = "Comida Tradicional",
                    Capacidad = 80,
                    Horario = "12:00 PM - 10:00 PM"
                },
                new Servicio
                {
                    Id = 3,
                    Nombre = "Tour Mirador del Inti",
                    Duracion = "2 horas",
                    PrecioTour = 50.00,
                    Horario = "8:00 AM - 5:00 PM"
                }
            };

            // Aplicar el filtro
            if (!string.IsNullOrEmpty(filtro))
            {
                if (filtro == "hotel")
                {
                    servicios = servicios.Where(s => s.Precio.HasValue).ToList();
                }
                else if (filtro == "restaurante")
                {
                    servicios = servicios.Where(s => !string.IsNullOrEmpty(s.TipoComida)).ToList();
                }
                else if (filtro == "tour")
                {
                    servicios = servicios.Where(s => !string.IsNullOrEmpty(s.Duracion)).ToList();
                }
            }

            var dashboardData = new SocioDashboardViewModel
            {
                NombreSocio = "Juan Pérez",
                TotalVisitas = 1200,
                ContactosRecibidos = 35,
                CalificacionPromedio = 4.2,
                Servicios = servicios
            };

            return View(dashboardData);
        }

        // Método para mostrar el formulario de edición del servicio
        public System.Web.Mvc.ActionResult Editar(int id)
        {
            // Aquí buscarías el servicio por su ID en la base de datos
            var servicio = ObtenerServicioPorId(id); // Simulación de obtener servicio

            if (servicio == null)
            {
                return HttpNotFound();
            }

            return View(servicio);
        }

        // Método para procesar el formulario de edición
        [System.Web.Mvc.HttpPost]
        public System.Web.Mvc.ActionResult Editar(Servicio servicio)
        {


            return View(servicio);
        }

        // Métodos de simulación (reemplaza con tu lógica de base de datos)
        private Servicio ObtenerServicioPorId(int id)
        {
            // Simulación de obtener el servicio por ID
            var servicios = new List<Servicio>
            {
                new Servicio
                {
                    Id = 1,
                    Nombre = "Hotel Pacific Suites",
                    Precio = 300.00,
                    NumeroHabitaciones = 45,
                    ServiciosIncluidos = "WiFi, Piscina, Desayuno"
                },
                new Servicio
                {
                    Id = 2,
                    Nombre = "Restaurante El Sabroso",
                    TipoComida = "Comida Tradicional",
                    Capacidad = 80,
                    Horario = "12:00 PM - 10:00 PM"
                },
                new Servicio
                {
                    Id = 3,
                    Nombre = "Tour Mirador del Inti",
                    Duracion = "2 horas",
                    PrecioTour = 50.00,
                    Horario = "8:00 AM - 5:00 PM"
                }
            };

            return servicios.FirstOrDefault(s => s.Id == id);
        }



        public async Task<System.Web.Mvc.ActionResult> EditarServicios(int negocioId)
        {
            System.Diagnostics.Debug.WriteLine("Negocio ID recibido en controlador: " + negocioId);

            var serviciosHotel = await ObtenerServiciosHotel(negocioId);
            var serviciosRestaurante = await ObtenerServiciosRestaurante(negocioId);
            var serviciosTurismo = await ObtenerServiciosTurismo(negocioId);

            var model = new Tuple<List<ServicioHotel>, List<ServicioRestaurante>, List<ServicioTurismo>>(
                serviciosHotel, serviciosRestaurante, serviciosTurismo);

            ViewBag.NegocioId = negocioId; // Asigna el negocioId a ViewBag

            return View(model);
        }
        private async Task<List<ServicioHotel>> ObtenerServiciosHotel(int negocioId)
        {
            var url = $"http://159.223.123.38:8000/api/hoteles/obtener_servicios?negocioId={negocioId}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ServicioHotel>>(json);
            }
            return new List<ServicioHotel>();
        }


        // Repite el mismo patrón de logging para las demás APIs de restaurantes y turismo


        private async Task<List<ServicioRestaurante>> ObtenerServiciosRestaurante(int negocioId)
        {
            var url = $"http://159.223.123.38:8000/api/restaurantes/obtener_servicios?negocioId={negocioId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ServicioRestaurante>>(json);
        }

        private async Task<List<ServicioTurismo>> ObtenerServiciosTurismo(int negocioId)
        {
            var url = $"http://159.223.123.38:8000/api/turismo/obtener_servicios?negocioId={negocioId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ServicioTurismo>>(json);
        }

        [System.Web.Mvc.HttpPost]
        public async Task<System.Web.Mvc.ActionResult> ActualizarServicioHotel(ServicioHotel model)
        {
            if (model == null)
            {
                TempData["ErrorMessage"] = "Error: No se recibieron datos válidos.";
                return RedirectToAction("EditarServicios", new { negocioId = model?.Id ?? 0 });
            }

            var url = "http://159.223.123.38:8000/api/hotel/actualizar_servicio";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Servicio de hotel actualizado correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error al actualizar el servicio de hotel.";
            }

            // Redirige a EditarServicios con el parámetro correcto.
            return RedirectToAction("EditarServicios", new { negocioId = model.NegocioId });
        }
        [System.Web.Mvc.HttpPost]
        public async Task<System.Web.Mvc.ActionResult> ActualizarServicioRestaurante(ServicioRestaurante model)
        {
            var url = "http://159.223.123.38:8000/api/restaurante/actualizar_servicio";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, jsonContent);

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Servicio de restaurante actualizado correctamente.";
            else
                TempData["ErrorMessage"] = "Error al actualizar el servicio de restaurante.";

            return RedirectToAction("EditarServicios", new { negocioId = model.NegocioId });
        }

        [System.Web.Mvc.HttpPost]
        public async Task<System.Web.Mvc.ActionResult> ActualizarServicioTurismo(ServicioTurismo model)
        {
            var url = "http://localhost:5000/api/turismo/actualizar_servicio";
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, jsonContent);

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Servicio de turismo actualizado correctamente.";
            else
                TempData["ErrorMessage"] = "Error al actualizar el servicio de turismo.";

            return RedirectToAction("EditarServicios", new { negocioId = model.NegocioId });
        }


    }
}

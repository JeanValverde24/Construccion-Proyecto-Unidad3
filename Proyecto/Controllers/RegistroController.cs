using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Proyecto.Models;

namespace Proyecto.Controllers
{
    public class RegistroController : Controller
    {
        

        public ActionResult Index()
        {
            return View();
        }
        
        private readonly HttpClient _httpClient;

        public RegistroController()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://159.223.123.38:8000/") };
        }
        [HttpGet]
        public async Task<ActionResult> RegistroNegocio()
        {
            ViewBag.IdSocio = Session["IdSocio"];
            // Cargar las listas para los menús desplegables
            ViewBag.TiposNegocio = await ObtenerTiposNegocioDesdeAPI();
            await Task.Delay(1000);  // Retraso de 1 segundo entre las solicitudes

            ViewBag.Provincias = await ObtenerProvinciasDesdeAPI();

         

            return View();
        }


        // Acción para procesar el registro de negocio
        [HttpPost]
        public async Task<ActionResult> RegistrarNegocio(Negocio negocio, IEnumerable<HttpPostedFileBase> TbImgRuta)
        {
            var url = "http://159.223.123.38:8000/api/registro/negocio"; // URL de la API Flask

            // Crear un objeto para enviar datos del negocio
            var requestData = new
            {
                TbNgcUsuario = negocio.TbNgcUsuario,
                TbNgcNombre = negocio.TbNgcNombre,
                TbNgcTipoNegocio = negocio.TbNgcTipoNegocio,
                TbNgcProvincia = negocio.TbNgcProvincia,
                TbNgcDireccion = negocio.TbNgcDireccion,
                TbNgcTelefono = negocio.TbNgcTelefono
            };

            using (var formContent = new MultipartFormDataContent())
            {
                // Convertir el negocio a JSON y añadirlo como parte del contenido del formulario
                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                formContent.Add(jsonContent, "negocio");

                // Añadir imágenes si existen
                if (TbImgRuta != null)
                {
                    foreach (var imagen in TbImgRuta)
                    {
                        if (imagen != null && imagen.ContentLength > 0)
                        {
                            byte[] fileData;
                            using (var binaryReader = new BinaryReader(imagen.InputStream))
                            {
                                fileData = binaryReader.ReadBytes(imagen.ContentLength);
                            }

                            var fileContent = new ByteArrayContent(fileData);
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(imagen.ContentType);
                            formContent.Add(fileContent, "TbImgRuta", imagen.FileName);
                        }
                    }
                }

                // Enviar la solicitud a la API Flask
                var response = await _httpClient.PostAsync(url, formContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("RegistroExitoso");
                }
                else
                {
                    TempData["Error"] = "Error en el registro del negocio. Intente nuevamente.";
                    return RedirectToAction("RegistroNegocio");
                }
            }
        }




        private async Task<IEnumerable<TipoNegocio>> ObtenerTiposNegocioDesdeAPI()
        {
            var response = await _httpClient.GetAsync("api/tipos_negocio");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<TipoNegocio>>(data);
            }
            return new List<TipoNegocio>(); // Retorna una lista vacía en caso de error
        }

        private async Task<IEnumerable<Provincia>> ObtenerProvinciasDesdeAPI()
        {
            var response = await _httpClient.GetAsync("api/provincias");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Provincia>>(data);
            }
            return new List<Provincia>(); // Retorna una lista vacía en caso de error
        }

        [HttpPost]
        public async Task<ActionResult> RegistrarTurista(Usuario usuario)
        {
            var url = "http://159.223.123.38:8000/api/registro/turista";
            var requestData = new
            {
                UsrNombresCompleto = usuario.UsrNombresCompleto,
                UsrCorreo = usuario.UsrCorreo,
                UsrDniRut = usuario.UsrDniRut,
                UsrContraseña = usuario.contraseña
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                int? usuarioId = jsonResponse?.id;

                // Guardar el ID en la sesión
                Session["IdUsuario"] = usuarioId ?? 0;

                TempData["Mensaje"] = "Registro exitoso. Bienvenido a la plataforma.";
                TempData["MensajeClase"] = "alert-success"; // Clase CSS para verde
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                string errorMessage = errorResponse?.error ?? "Error desconocido";

                TempData["Mensaje"] = errorMessage;
                TempData["MensajeClase"] = "alert-danger"; // Clase CSS para rojo
                return RedirectToAction("Index", "Registro");
            }
        }


        [HttpPost]
        public async Task<ActionResult> RegistrarSocio(Usuario usuario)
        {
            var url = "http://159.223.123.38:8000/api/registro/socio";
            var requestData = new
            {
                UsrNombresCompleto = usuario.UsrNombresCompleto,
                UsrCorreo = usuario.UsrCorreo,
                UsrRuc = usuario.UsrRuc,
                UsrContraseña = usuario.contraseña
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                int? socioId = jsonResponse?.id;

                // Guardar el ID en la sesión
                Session["IdUsuario"] = socioId ?? 0;
                Session["Administrador"] = 1;

                TempData["Mensaje"] = "Registro exitoso. Acceso al panel de administración.";
                TempData["MensajeClase"] = "alert-success"; // Clase CSS para verde
                return RedirectToAction("Index", "Administrador");
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                string errorMessage = errorResponse?.error ?? "Error desconocido";

                if (errorMessage.Contains("El RUC ya está registrado"))
                {
                    TempData["Mensaje"] = "El RUC ya está registrado.";
                }
                else
                {
                    TempData["Mensaje"] = "Error en el registro. Inténtelo nuevamente.";
                }

                TempData["MensajeClase"] = "alert-danger"; // Clase CSS para rojo
                return RedirectToAction("Index", "Registro");
            }
        }



    }
}

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
    public class EstadisticaController : Controller
    {
        private readonly string apiBaseUrl = "http://159.223.123.38:8000/api"; // Cambia por la URL de tu API

        // Método para mostrar la vista inicial de estadísticas
        public async Task<ActionResult> Index()
        {
            // Obtener el ID del usuario de la sesión
            int userId = Convert.ToInt32(Session["IdUsuario"]);

            // Consumir la API de negocios
            List<NegocioViewModel> negocios = await ObtenerNegocios(userId);
            return View(negocios); // Pasar los datos a la vista
        }

        // Método para obtener estadísticas de un negocio
        public async Task<ActionResult> EstadisticasPorNegocio(int negocioId)
        {
            EstadisticasViewModel estadisticas = await ObtenerEstadisticas(negocioId);
            return Json(estadisticas, JsonRequestBehavior.AllowGet); // Devuelve estadísticas como JSON
        }

        // Método privado para consumir la API de negocios
        private async Task<List<NegocioViewModel>> ObtenerNegocios(int userId)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{apiBaseUrl}/negocios?user_id={userId}");
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<NegocioViewModel>>(responseData);
                }
                else
                {
                    throw new Exception("Error al obtener los negocios.");
                }
            }
        }

        // Método privado para consumir la API de estadísticas
        private async Task<EstadisticasViewModel> ObtenerEstadisticas(int negocioId)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{apiBaseUrl}/estadisticas/{negocioId}");
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<EstadisticasViewModel>(responseData);
                }
                else
                {
                    throw new Exception("Error al obtener las estadísticas.");
                }
            }
        }
    }

    // Modelos de datos
    public class NegocioViewModel
    {
        public int NegocioId { get; set; }
        public int negocio_id { get; set; }

        public string TbNgcNombre { get; set; }
        public string negocio_nombre { get; set; }

        public string Provincia { get; set; }
        public string TipoNegocio { get; set; }
        public string tipo_negocio { get; set; }

        public string TbNgcDireccion { get; set; }
        public string TbNgcTelefono { get; set; }
    }

    public class EstadisticasViewModel
    {
        public List<Recomendacion> Recomendaciones { get; set; }
        public int Visitas { get; set; }
    }

    public class Recomendacion
    {
        public string TipoServicio { get; set; }
        public int Cantidad { get; set; }
    }

}
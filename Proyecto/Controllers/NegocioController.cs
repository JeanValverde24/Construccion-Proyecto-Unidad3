using Proyecto.Models;
using System.Collections.Generic;
using Proyecto.Models;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http;

public class NegocioController : Controller
{
    private readonly HttpClient _httpClient;

    public NegocioController()
    {
        _httpClient = new HttpClient();
    }
    // Método para obtener la lista de servicios
    private List<Servicio> ObtenerServicios()
    {
        // Simulación de datos
        return new List<Servicio>
        {
            new Servicio { Nombre = "Servicio 1", Precio = 100 },
            new Servicio { Nombre = "Servicio 2", Precio = 200 }
        };
    }

    // Método que devuelve la vista de gestión de negocio
    public ActionResult GestionarNegocio()
    {
        var servicios = ObtenerServicios(); // Obtener los servicios
        return View(servicios); // Pasar el modelo a la vista
    }
    public async Task<System.Web.Mvc.ActionResult> Negocios()
    {
        var userId = Session["IdUsuario"];
        System.Diagnostics.Debug.WriteLine($"Valor de IdUsuario en la sesión: {Session["IdUsuario"]}");
        if (userId == null)
        {
            return RedirectToAction("Index", "Account");
        }

        var url = $"http://159.223.123.38:8000/api/negocios?user_id={userId}";
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


}

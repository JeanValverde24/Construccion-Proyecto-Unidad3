using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;

namespace Proyecto.Controllers
{
    public class SocioController : Controller
    {
        // GET: AñadirServicio
        public ActionResult AñadirServicio()
        {
            return View();
        }

        // GET: Dashboard con filtro
        public ActionResult Dashboard(string filtro = "")
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
        public ActionResult Editar(int id)
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
        [HttpPost]
        public ActionResult Editar(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                // Actualizar el servicio en la base de datos
                ActualizarServicio(servicio); // Simulación de actualización

                return RedirectToAction("Dashboard");
            }

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

        private void ActualizarServicio(Servicio servicio)
        {
            // Aquí pondrías la lógica para actualizar el servicio en la base de datos.
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class SocioDashboardViewModel
    {
        public string NombreSocio { get; set; }
        public int TotalVisitas { get; set; }
        public int ContactosRecibidos { get; set; }
        public double CalificacionPromedio { get; set; }
        public List<Servicio> Servicios { get; set; }
    }

    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Características de Hotel
        public double? Precio { get; set; }  // Precio por noche
        public int? NumeroHabitaciones { get; set; }
        public string ServiciosIncluidos { get; set; }

        // Características de Restaurante
        public string TipoComida { get; set; }
        public int? Capacidad { get; set; }
        public string Horario { get; set; }

        // Características de Tour o Lugar Turístico
        public string Duracion { get; set; }  // Duración del tour
        public double? PrecioTour { get; set; }  // Precio del tour

    }

}
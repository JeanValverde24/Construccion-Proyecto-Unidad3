using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class HistorialServicio
    {
        public string NombreLocal { get; set; }
        public DateTime Fecha { get; set; }
        public string Servicio { get; set; }
        public bool Calificado { get; set; }
    }

}
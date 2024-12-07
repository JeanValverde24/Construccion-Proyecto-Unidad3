using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class ServicioTurismo
    {
        public int NegocioId { get; set; } // ID del negocio, no debe ser null
        public string NombreLugar { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Provincia { get; set; } // Ej. Tacna, Tarata, etc.
    }
}

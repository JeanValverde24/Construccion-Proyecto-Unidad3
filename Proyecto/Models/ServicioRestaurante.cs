using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public enum TipoPlatoEnum
    {
        bebida = 1,
        postre = 2,
        desayuno = 3,
        entrada = 4,
        almuerzo = 5,
        cena = 6
    }

    public class ServicioRestaurante
    {
        public int NegocioId { get; set; } // ID del negocio, no debe ser null
        public string NombrePlato { get; set; }
        public TipoPlatoEnum TipoPlato { get; set; }
        public decimal Precio { get; set; }

    }
}

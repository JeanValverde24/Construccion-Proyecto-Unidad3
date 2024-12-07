using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class TipoNegocio
    {
        public int TpNgcId { get; set; }        // ID del tipo de negocio, clave primaria
        public string Nombre { get; set; }      // Nombre o descripción del tipo de negocio
    }
}
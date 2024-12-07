using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class DatosEstadisticas
    {
        public List<int> VisitasPorSemana { get; set; }
        public int IntentosExitosos { get; set; }
        public int TotalIntentos { get; set; }
        public List<decimal> VentasPorMes { get; set; }
    }
}
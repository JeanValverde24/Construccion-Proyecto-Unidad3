using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class CotizacionResponse
    {
        public double AjusteEspecial { get; set; }
        public double CostoHotel { get; set; }
        public double CostoLugar { get; set; }
        public double CostoRestaurante { get; set; }
        public string Hotel { get; set; }
        public string Lugar { get; set; }
        public double PorcentajePresupuesto { get; set; }
        public double PresupuestoRestante { get; set; }
        public string Restaurante { get; set; }
        public double Total { get; set; }
    }


}
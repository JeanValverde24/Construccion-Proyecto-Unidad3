using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class ServicioHotel
    {
        public int NegocioId { get; set; } // Id del negocio al que se le añadirá el cuarto
        public int Id { get; set; } // Id del negocio al que se le añadirá el cuarto

        public int CantidadPersonas { get; set; }
        public string Wifi { get; set; }
        public string AguaCaliente { get; set; }
        public string RoomService { get; set; }
        public string Cochera { get; set; }
        public string Cable { get; set; }
        public string DesayunoIncluido { get; set; }
        public decimal Precio { get; set; }
        public string Fotos { get; set; }
    }
}
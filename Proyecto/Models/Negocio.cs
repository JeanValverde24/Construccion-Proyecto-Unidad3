using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class Negocio
    {

        public int? negocio_id { get; set; }
        public int TbNgcUsuario { get; set; }
        public int TbNgcTipoNegocio { get; set; }
        public int TbNgcProvincia { get; set; }

        public string TbNgcNombre { get; set; }

        public string TipoNegocio { get; set; }

        public string Provincia { get; set; }

        public string TbNgcDireccion { get; set; }

        public string TbNgcTelefono { get; set; }

        public List<string> ImagenesUrl { get; set; } = new List<string>();
        public List<string> Imagenes { get; set; } = new List<string>();

        public List<Feedback> Feedbacks { get; set; } // Opcional




    }



}
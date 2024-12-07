using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class NegocioConFeedback
    {
        public int negocio_id { get; set; }
        public string negocio_nombre { get; set; }
        public string provincia { get; set; }
        public string tipo_negocio { get; set; }
        public List<Feedback> feedbacks { get; set; }
    }

    public class Feedbacks
    {
        public int FbId { get; set; }
        public int FbUsuarioId { get; set; }
        public int FbCalificacion { get; set; }
        public string FbComentario { get; set; }
        public DateTime FbFecha { get; set; }
    }
}
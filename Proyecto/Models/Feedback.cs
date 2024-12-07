using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Comentario { get; set; }
        public int Estrellas { get; set; }
        public DateTime Fecha { get; set; }
        public int NegocioId { get; set; }
        public string NegocioNombre { get; set; }
        public string FbComentario { get; set; }
        public int FbCalificacion { get; set; }
        public DateTime FbFecha { get; set; }
    }

}
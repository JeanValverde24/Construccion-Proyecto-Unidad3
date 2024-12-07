using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class ImagenNegocio
    {
        public int TbImgId { get; set; }          // ID de la imagen, clave primaria
        public int TbImgNegocioId { get; set; }    // ID del negocio asociado, clave foránea
        public string TbImgRuta { get; set; }      // Ruta de la imagen almacenada en el servidor

        // Relación con el modelo `TbNegocio`, si tienes un modelo de negocio para referencia directa
        public virtual Negocio Negocio { get; set; }
    }
}
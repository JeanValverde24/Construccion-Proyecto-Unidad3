using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsrId { get; set; }

        [StringLength(8, ErrorMessage = "El DNI/RUT no puede tener más de 8 caracteres.")]
        public string UsrDniRut { get; set; }

        [StringLength(11, ErrorMessage = "El RUC no puede tener más de 11 caracteres.")]
        public string UsrRuc { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50)]
        public string UsrNombresCompleto { get; set; }

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        [StringLength(50)]
        public string UsrApellidoPaterno { get; set; }

        [Required(ErrorMessage = "El apellido materno es obligatorio.")]
        [StringLength(50)]
        public string UsrApellidoMaterno { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico no válido.")]
        [StringLength(200)]
        public string UsrCorreo { get; set; }

        [Required(ErrorMessage = "El país es obligatorio.")]
        [StringLength(10)]
        public string UsrPais { get; set; }

        [StringLength(13, ErrorMessage = "El número de teléfono no puede tener más de 13 caracteres.")]
        public string UsrNumero { get; set; }

        [ForeignKey("TipoUsuario")]
        public int? UsrTipoUsuario { get; set; }

        [Required]
        public int UsrEstado { get; set; }
        
        public string contraseña { get; set; }
        // Propiedad de navegación
        public virtual int TipoUsuario { get; set; }
    }

}
using System.ComponentModel.DataAnnotations;

namespace Gestion_Biblioteca.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un correo válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria")]
        public string Clave { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CustomerWebApi.Dtos.Request
{
    public class CustomerFilterRequest : PagedRequest
    {
        [StringLength(100, ErrorMessage = "Name no puede exceder 100 caracteres")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Email debe ser un correo válido")]
        [StringLength(256, ErrorMessage = "Email no puede exceder 256 caracteres")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Phone debe ser un teléfono válido")]
        [StringLength(20, ErrorMessage = "Phone no puede exceder 20 caracteres")]
        public string Phone { get; set; }
    }
}

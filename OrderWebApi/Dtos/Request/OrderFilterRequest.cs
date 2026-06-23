using System.ComponentModel.DataAnnotations;

namespace OrderWebApi.Dtos.Request
{
    public class OrderFilterRequest : PagedRequest
    {
        [StringLength(50, ErrorMessage = "CustomerId no puede exceder 50 caracteres")]
        public string CustomerId { get; set; }

        [StringLength(50, ErrorMessage = "Status no puede exceder 50 caracteres")]
        public string Status { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}

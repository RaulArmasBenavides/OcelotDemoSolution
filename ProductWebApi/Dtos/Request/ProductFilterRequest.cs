using System.ComponentModel.DataAnnotations;

namespace ProductWebApi.Dtos.Request
{
    public class ProductFilterRequest : PagedRequest
    {
        [StringLength(100, ErrorMessage = "Name no puede exceder 100 caracteres")]
        public string Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "PriceMin debe ser mayor o igual a 0")]
        public decimal? PriceMin { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "PriceMax debe ser mayor o igual a 0")]
        public decimal? PriceMax { get; set; }

        [StringLength(50, ErrorMessage = "Category no puede exceder 50 caracteres")]
        public string Category { get; set; }
    }
}

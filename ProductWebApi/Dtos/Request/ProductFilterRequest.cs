namespace ProductWebApi.Dtos.Request
{
    public class ProductFilterRequest : PagedRequest
    {
        public string Name { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public string Category { get; set; }
    }
}

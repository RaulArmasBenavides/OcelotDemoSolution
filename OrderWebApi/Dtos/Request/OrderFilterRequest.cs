namespace OrderWebApi.Dtos.Request
{
    public class OrderFilterRequest : PagedRequest
    {
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}

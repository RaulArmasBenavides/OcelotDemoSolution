namespace CustomerWebApi.Dtos.Request
{
    public class CustomerFilterRequest : PagedRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

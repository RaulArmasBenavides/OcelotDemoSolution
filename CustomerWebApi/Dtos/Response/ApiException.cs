namespace CustomerWebApi.Dtos.Response
{
    public class ApiException : ApiResponse
    {
        public ApiException(int StatusCode, string message = null, string details = null) : base(StatusCode, message)
        {
            details = Details;
        }
        public string Details { get; set; }
    }
}

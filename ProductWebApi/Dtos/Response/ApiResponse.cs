namespace ProductWebApi.Dtos.Response
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ApiResponse(int StatusCode, string message = null)
        {
            this.StatusCode = StatusCode;
            this.Message = message ?? DefaultStatusCodeMessage(StatusCode);
        }

        private string DefaultStatusCodeMessage(int StatusCode)
        {
            return StatusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                404 => "Not Found",
                500 => "Internal Server Error",
                0 => "Something went wrong",
                _ => "Unknown error"
            };
        }
    }
}

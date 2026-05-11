namespace CustomerWebApi.Dtos.Response
{

    /// <summary>
    /// Api Response generic
    /// </summary>
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ApiResponse(int StatusCode, string message = null)
        {
            this.StatusCode = StatusCode;
            this.Message = message ?? this.DefaultStatusCodeMessage(StatusCode);
        }
        private string DefaultStatusCodeMessage(int StatusCode)
        {
            return StatusCode switch
            {
                400 => "A bad request you have made",
                401 => "Authorized you have not",
                404 => "Resource Found it was not",
                500 => "Errors are the path to the dark side.  Errors lead to anger.   Anger leads to hate.  Hate leads to career change.",
                0 => "Some Thing Went Wrong",
                _ => throw new NotImplementedException()
            };
        }
    }
}

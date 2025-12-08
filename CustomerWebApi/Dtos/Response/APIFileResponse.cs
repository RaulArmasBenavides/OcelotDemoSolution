namespace CustomerWebApi.Dtos.Response
{
    public class APIFileResponse
    {
        public APIFileResponse()
        {
            this.StatusCode = false;

        }
        public bool StatusCode { get; set; }


        public Stream File { get; set; }
    }
}

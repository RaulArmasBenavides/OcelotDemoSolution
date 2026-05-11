using CustomerWebApi.Dtos.Response;
using System.Net;
using System.Text.Json;

namespace CustomerWebApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment hostEnvironment)
        {
            _next = next;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                // Obtener el mensaje de la excepción.
                var message = ex.Message;

                // Obtener el nombre completo del tipo de la excepción.
                var exceptionType = ex.GetType().FullName;

                // Obtener la traza de la pila.
                var stackTrace = ex.StackTrace;
                // Acceder a los valores de la ruta
                var routeValues = httpContext.Request.RouteValues;

                // Obtener el controlador y la acción, si están disponibles
                var controller = routeValues["controller"]?.ToString();
                var action = routeValues["action"]?.ToString();
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _hostEnvironment.IsDevelopment() ?
                    new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                    : new ApiException((int)HttpStatusCode.InternalServerError);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}

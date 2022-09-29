using static PdfConverter.API.Models.ResponseDtos.GenericResponse;

namespace PdfConverter.API.Middlewares
{
    public class OperationCanceledMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OperationCanceledMiddleware> _logger;
        public OperationCanceledMiddleware(
            RequestDelegate next,
            ILogger<OperationCanceledMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request was cancelled");
                context.Response.StatusCode = 409;
            }
            catch(Exception ex) 
            {
                _logger.LogInformation("Internal Server Error");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new ErrorResponse()
                {
                    StatusCode = 500,
                    Message = "Internal Server Error",
                    Success = false
                });
            }
        }
    }
}

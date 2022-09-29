namespace PdfConverter.API.Models.ResponseDtos
{
    public class GenericResponse
    {
        public class SuccessResponse<T>
        {
            public string? Message { get; set; } = "Success";
            public int StatusCode { get; set; } = StatusCodes.Status200OK;
            public bool Success { get; set; } = true;
            public T? Data { get; set; }
        }

        public class ErrorResponse
        {
            public string? Message { get; set; } = "Internal Server Error";
            public int StatusCode { get; set; } = StatusCodes.Status500InternalServerError;

            public bool Success { get; set; } = false;
        }
    }
}

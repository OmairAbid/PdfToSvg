namespace PdfConverter.API.Models.ResponseDtos
{
    public class UploadPdfResponseDto
    {
        public string? Content { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
        public int PageNo { get; set; }
        public string? RedirectUrl { get; set; }
         
    }
}

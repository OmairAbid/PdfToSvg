using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfConverter.API.Models.ResponseDtos;
using Shared;
using System.Net;
using System.Net.Http.Headers;
using System.Xml;
using static PdfConverter.API.Models.ResponseDtos.GenericResponse;

namespace PdfConverter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SvgController : ControllerBase
    {
        private readonly ILogger<SvgController> _logger;
        private readonly string svgFolderName = Path.Combine("Resources", "Svgs");

        public SvgController(ILogger<SvgController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{fileName}/{pageNo}", Name = "GetSvg")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> Get(string fileName, int pageNo, CancellationToken token)
        {
            var SvgPath = Path.Combine(Directory.GetCurrentDirectory(), svgFolderName, fileName + "UsingConvertABC\\");

            XmlDocument docLoad = new XmlDocument();
            docLoad.Load(SvgPath + $"{pageNo}.svg");

            var response = new UploadPdfResponseDto
            {
                Content = docLoad.OuterXml,
                ContentType = "svg",
                FileName = fileName,
                PageNo = pageNo,
            };


            return Ok(new SuccessResponse<UploadPdfResponseDto> { Data = response });
        }

        [HttpPost, DisableRequestSizeLimit]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> Upload(CancellationToken token)
        {

            var form = await Request.ReadFormAsync(token);
            var file = form.Files.First();

            var pdfFolderName = Path.Combine("Resources", "Pdfs");

            var fileName = string.Empty;
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), pdfFolderName);
            if (file.Length > 0)
            {
                fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    token.ThrowIfCancellationRequested();
                    file.CopyTo(stream);
                }

                var filePath = Path.Combine(pathToSave, fileName);
                string page = await PdfUtility.ConvertUsingAbcPdf(Directory.GetCurrentDirectory(), svgFolderName, filePath, fileName, token);

                //var createdResource = new { Filename = fileName, Page = "1" };
                //var actionName = "GetSvg";
                //var routeValues = new { filename = createdResource.Filename, page = createdResource.Page };

                var response = new UploadPdfResponseDto
                {
                    Content = page,
                    ContentType = "svg",
                    FileName = fileName,
                    PageNo = 1,
                    RedirectUrl = "api/Svg/Get"
                };

                //return CreatedAtRoute(actionName, routeValues, createdResource);
                return Ok(new SuccessResponse<UploadPdfResponseDto> { Data = response });
            }
            else
            {
                return BadRequest(new ErrorResponse()
                {
                    Message = "File is empty",
                    StatusCode = (int)HttpStatusCode.BadRequest
                });
            }

        }
    }
}

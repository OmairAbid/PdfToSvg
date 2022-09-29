using Microsoft.AspNetCore.Http.Features;

namespace PdfConverter.API.Extensions
{
    public static class ConfigureServices
    {
        public static IServiceCollection FormOptionConfigure(this IServiceCollection services)
        {
            services.Configure<FormOptions>(opt =>
            {
                opt.ValueLengthLimit = int.MaxValue;
                opt.MultipartBodyLengthLimit = int.MaxValue;
                opt.MemoryBufferThreshold = int.MaxValue;
            });
            return services;
        }
    }
}

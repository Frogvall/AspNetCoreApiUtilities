using Frogvall.AspNetCore.ApiUtilities.ExceptionHandling;
using Frogvall.AspNetCore.ApiUtilities.Mapper;
using Frogvall.AspNetCore.ApiUtilities.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApiExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new ApiExceptionHandler(
                    builder.ApplicationServices.GetRequiredService<IExceptionMapper>(),
                    builder.ApplicationServices.GetRequiredService<IHostingEnvironment>(),
                    builder.ApplicationServices.GetRequiredService<ILogger<ApiExceptionHandler>>())
                    .ExceptionHandler
            });
        }

        public static IApplicationBuilder UseExceptionStatusCodeDecorator(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionStatusCodeDecoratorMiddleware>();
        }
    }
}

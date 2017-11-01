using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Frogvall.AspNetCore.ApiUtilities.ExceptionHandling
{
    public class ApiExceptionHandler
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<ApiExceptionHandler> _logger;
        private readonly JsonSerializer _serializer;

        public const string DefaultErrorMessage = "A server error occurred.";

        public ApiExceptionHandler(IHostingEnvironment env, ILogger<ApiExceptionHandler> logger)
        {
            _env = env;
            _logger = logger;
            _serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task ExceptionHandler(HttpContext context)
        {            
            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (ex == null) return;

            context.Response.ContentType = "application/json";

            if (ex is ApiException)
            {
                var statusCode = (int) (ex as ApiException).StatusCode;
                context.Response.StatusCode = statusCode;
                _logger.LogInformation("ApiException caught by ApiExceptionHandler with {exception} and {statusCode}", ex, statusCode);
            }
            else
            {
                var statusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.StatusCode = statusCode;
                _logger.LogWarning("Unhandled exception caught by ApiExceptionHandler with {exception} and {statusCode}", ex, statusCode);
            }

            var error = BuildError(ex, _env);

            using (var writer = new StreamWriter(context.Response.Body))
            {
                _serializer.Serialize(writer, error);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }

        private static ApiError BuildError(Exception ex, IHostingEnvironment env)
        {
            var error = new ApiError();

            if (env.IsDevelopment())
            {
                error.Message = ex.Message;
                error.Detail = ex.StackTrace;
            }
            else
            {
                error.Message = DefaultErrorMessage;
                error.Detail = ex.Message;
            }

            return error;
        }
    }
}

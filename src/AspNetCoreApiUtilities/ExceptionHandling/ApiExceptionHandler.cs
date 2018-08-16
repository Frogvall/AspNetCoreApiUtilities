using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AsyncFriendlyStackTrace;
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

            HttpStatusCode statusCode;

            if (ex is ApiException)
            {
                statusCode = (ex as ApiException).StatusCode;
                context.Response.StatusCode = (int)statusCode;
                _logger.LogInformation(ex, "ApiException caught by ApiExceptionHandler with {statusCode}.", statusCode);
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;
                context.Response.StatusCode = (int)statusCode;
                _logger.LogError(ex, "Unhandled exception caught by ApiExceptionHandler.");
            }

            var error = CreateApiError(ex, _env, statusCode);

            using (var writer = new StreamWriter(context.Response.Body))
            {
                _serializer.Serialize(writer, error);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }

        private static ApiError CreateApiError(Exception ex, IHostingEnvironment env, HttpStatusCode statusCode)
        {
            var error = new ApiError();

            if (env.IsDevelopment())
            {
                error.Message = ex.Message;
                error.Detail = ex.ToAsyncString();
            }
            else
            {
                error.Message = Regex.Replace(statusCode.ToString(), "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
                error.Detail = ex.Message;
            }

            return error;
        }
    }
}

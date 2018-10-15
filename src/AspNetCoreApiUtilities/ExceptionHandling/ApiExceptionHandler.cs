using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AsyncFriendlyStackTrace;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;
using Frogvall.AspNetCore.ApiUtilities.Mapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Frogvall.AspNetCore.ApiUtilities.ExceptionHandling
{
    internal class ApiExceptionHandler
    {
        private readonly IExceptionMapper _mapper;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<ApiExceptionHandler> _logger;
        private readonly JsonSerializer _serializer;

        internal ApiExceptionHandler(IExceptionMapper mapper, IHostingEnvironment env, ILogger<ApiExceptionHandler> logger)
        {
            _mapper = mapper;
            _env = env;
            _logger = logger;
            _serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        internal async Task ExceptionHandler(HttpContext context)
        {
            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (ex == null) return;

            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode;
            int errorCode;
            object developerContext = null;

            switch (ex)
            {
                case BaseApiException _:
                    try
                    {
                        developerContext = (ex as BaseApiException)?.DeveloperContext;
                        errorCode = _mapper.GetErrorCode(ex.GetType());
                        var exceptionType = _mapper.GetExceptionReturnType(ex.GetType());
                        switch (exceptionType)
                        {
                            case ExceptionReturnType.Error:
                                statusCode = HttpStatusCode.BadRequest;
                                context.Response.StatusCode = (int)statusCode;
                                _logger.LogDebug(ex, "Mapped BaseApiException of type {exceptionType} caught by ApiExceptionHandler. Handled: {handled}", ex.GetType(), true);
                                break;
                            case ExceptionReturnType.Fault:
                                statusCode = HttpStatusCode.InternalServerError;
                                context.Response.StatusCode = (int)statusCode;
                                _logger.LogDebug(ex, "Mapped BaseApiException of type {exceptionType} caught by ApiExceptionHandler. Handled: {handled}", ex.GetType(), true);
                                break;
                            default:
                                statusCode = HttpStatusCode.InternalServerError;
                                context.Response.StatusCode = (int)statusCode;
                                _logger.LogDebug(ex, "Unmapped BaseApiException of type {exceptionType} caught by ApiExceptionHandler. Handled: {handled}", ex.GetType(), false);
                                break;
                        }
                    }
                    catch (ArgumentException)
                    {
                        goto default;
                    }

                    break;
                case ApiException _:
                    errorCode = -2;
                    statusCode = (ex as ApiException).StatusCode;
                    context.Response.StatusCode = (int)statusCode;
                    _logger.LogInformation(ex, "ApiException caught by ApiExceptionHandler with {statusCode}. Handled: {handled}", statusCode, true);
                    break;
                default:
                    errorCode = -1;
                    statusCode = HttpStatusCode.InternalServerError;
                    context.Response.StatusCode = (int)statusCode;
                    _logger.LogError(ex, "Unhandled exception of type {exceptionType} caught by ApiExceptionHandler. Handled: {handled}", ex.GetType(), false);
                    break;
            }

            var error = new ApiError
            {
                CorrelationId = context.TraceIdentifier,
                DeveloperContext = developerContext,
                ErrorCode = errorCode,
            };

            if (_env.IsDevelopment())
            {
                error.Message = ex.Message;
                error.DetailedMessage = ex.ToAsyncString();
            }
            else
            {
                error.Message = Regex.Replace(statusCode.ToString(), "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
                error.DetailedMessage = ex.Message;
            }

            using (var writer = new StreamWriter(context.Response.Body))
            {
                _serializer.Serialize(writer, error);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
    }
}

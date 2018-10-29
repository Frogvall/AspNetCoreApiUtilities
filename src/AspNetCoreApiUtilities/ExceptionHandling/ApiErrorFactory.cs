using System;
using System.Net;
using System.Text.RegularExpressions;
using AsyncFriendlyStackTrace;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;
using Frogvall.AspNetCore.ApiUtilities.Mapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Frogvall.AspNetCore.ApiUtilities.ExceptionHandling
{
    public static class ApiErrorFactory
    {
        internal static ApiError Build<TCategoryName>(HttpContext context, Exception ex, IExceptionMapper mapper,
            ILogger<TCategoryName> logger, bool isDevelopment)
        {
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
                        errorCode = mapper.GetErrorCode(ex as BaseApiException);
                        var exceptionType = mapper.GetExceptionReturnType(ex as BaseApiException);
                        switch (exceptionType)
                        {
                            case ExceptionReturnType.Error:
                                statusCode = HttpStatusCode.BadRequest;
                                context.Response.StatusCode = (int)statusCode;
                                logger.LogDebug(ex,
                                    "Mapped BaseApiException of type {exceptionType} caught by ApiExceptionHandler. Unexpected: {unexpected}",
                                    ex.GetType(), false);
                                break;
                            case ExceptionReturnType.Fault:
                                statusCode = HttpStatusCode.InternalServerError;
                                context.Response.StatusCode = (int)statusCode;
                                logger.LogDebug(ex,
                                    "Mapped BaseApiException of type {exceptionType} caught by ApiExceptionHandler. Unexpected: {unexpected}",
                                    ex.GetType(), false);
                                break;
                            default:
                                statusCode = HttpStatusCode.InternalServerError;
                                context.Response.StatusCode = (int)statusCode;
                                logger.LogDebug(ex,
                                    "BaseApiException of unknown type {exceptionType} caught by ApiExceptionHandler. Unexpected: {unexpected}",
                                    ex.GetType(), true);
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
                    logger.LogInformation(ex,
                        "ApiException caught by ApiExceptionHandler with {statusCode}. Unexpected: {unexpected}", statusCode, false);
                    break;
                default:
                    errorCode = -1;
                    statusCode = HttpStatusCode.InternalServerError;
                    context.Response.StatusCode = (int)statusCode;
                    logger.LogError(ex,
                        "Unhandled exception of type {exceptionType} caught by ApiExceptionHandler. Unexpected: {unexpected}",
                        ex.GetType(), true);
                    break;
            }

            var error = new ApiError
            {
                CorrelationId = context.TraceIdentifier,
                DeveloperContext = developerContext,
                ErrorCode = errorCode,
            };

            if (isDevelopment)
            {
                error.Message = ex.Message;
                error.DetailedMessage = ex.ToAsyncString();
            }
            else
            {
                error.Message = Regex.Replace(statusCode.ToString(), "[a-z][A-Z]",
                    m => m.Value[0] + " " + char.ToLower(m.Value[1]));
                error.DetailedMessage = ex.Message;
            }

            return error;
        }
    }
}
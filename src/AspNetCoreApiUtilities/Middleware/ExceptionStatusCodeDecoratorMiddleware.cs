using System;
using System.Net;
using System.Threading.Tasks;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;
using Frogvall.AspNetCore.ApiUtilities.Mapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Frogvall.AspNetCore.ApiUtilities.Middleware
{
    public class ExceptionStatusCodeDecoratorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionMapper _mapper;
        private readonly ILogger<ExceptionStatusCodeDecoratorMiddleware> _logger;

        public ExceptionStatusCodeDecoratorMiddleware (RequestDelegate next, IExceptionMapper mapper, ILogger<ExceptionStatusCodeDecoratorMiddleware> logger)
        {
            _next = next;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BaseApiException ex)
            {
                var exceptionReturnType = _mapper.GetExceptionReturnType(ex);
                switch (exceptionReturnType)
                {
                    case ExceptionReturnType.Error:
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        _logger.LogDebug(ex, "Mapped BaseApiException of type {ExceptionType} caught, decorating response status code: {StatusCode}.", ex.GetType(), HttpStatusCode.BadRequest.ToString());
                        break;
                    case ExceptionReturnType.Fault:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        _logger.LogDebug(ex, "Mapped BaseApiException of type {ExceptionType} caught, decorating response status code: {StatusCode}.", ex.GetType(), HttpStatusCode.InternalServerError.ToString());
                        break;
                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        _logger.LogDebug(ex, "Unmapped BaseApiException of type {ExceptionType} caught, decorating response status code: {StatusCode}.", ex.GetType(), HttpStatusCode.InternalServerError.ToString());
                        break;
                }
                throw;
            }
            catch (ApiException ex)
            {
                _logger.LogDebug(ex, "ApiException caught, decorating response status code: {StatusCode}.", ex.StatusCode.ToString());
                context.Response.StatusCode = (int)ex.StatusCode;
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Exception caught, decorating response status code: {StatusCode}.", HttpStatusCode.InternalServerError.ToString());
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                throw;
            }
        }
    }
}
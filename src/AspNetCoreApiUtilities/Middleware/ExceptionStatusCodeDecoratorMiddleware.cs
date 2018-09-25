using System;
using System.Net;
using System.Threading.Tasks;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Frogvall.AspNetCore.ApiUtilities.Middleware
{
    public class ExceptionStatusCodeDecoratorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionStatusCodeDecoratorMiddleware> _logger;

        public ExceptionStatusCodeDecoratorMiddleware (RequestDelegate next, ILogger<ExceptionStatusCodeDecoratorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                _logger.LogDebug(ex, $"ApiException caught, decorating response status code: {ex.StatusCode.ToString()}.");
                context.Response.StatusCode = (int)ex.StatusCode;
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, $"Exception caught, decorating response status code: {HttpStatusCode.InternalServerError.ToString()}.");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                throw;
            }
        }
    }
}
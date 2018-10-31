﻿using System;
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
                var statusCode = _mapper.GetExceptionHandlerReturnCode(ex);
                _logger.LogDebug(ex, "Mapped BaseApiException of type {ExceptionType} caught, decorating response status code: {StatusCode}.", ex.GetType(), statusCode.ToString());
                context.Response.StatusCode = (int)statusCode;
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
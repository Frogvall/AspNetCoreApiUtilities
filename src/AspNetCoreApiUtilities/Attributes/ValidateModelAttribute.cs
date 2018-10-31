using Frogvall.AspNetCore.ApiUtilities.ExceptionHandling;
using Frogvall.AspNetCore.ApiUtilities.Mapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Frogvall.AspNetCore.ApiUtilities.Attributes
{
    public sealed class ValidateModelAttribute : ActionFilterAttribute
    {
        public int ErrorCode { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var mapper = context.HttpContext.RequestServices.GetService<IExceptionMapper>();
                context.Result = new BadRequestObjectResult(new ApiError(ErrorCode, context.ModelState, context.HttpContext.TraceIdentifier, mapper.Options.ServiceName));
            }
        }
    }
}
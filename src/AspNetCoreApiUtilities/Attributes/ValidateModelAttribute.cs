using Frogvall.AspNetCore.ApiUtilities.ExceptionHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Frogvall.AspNetCore.ApiUtilities.Attributes
{
    public sealed class ValidateModelAttribute : ActionFilterAttribute
    {
        public int ErrorCode { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new ApiError(ErrorCode, context.ModelState, context.HttpContext.TraceIdentifier));
            }
        }
    }
}
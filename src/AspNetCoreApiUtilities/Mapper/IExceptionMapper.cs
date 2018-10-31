using System.Net;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;

namespace Frogvall.AspNetCore.ApiUtilities.Mapper
{
    public interface IExceptionMapper
    {
        ExceptionMapperOptions Options { get; }
        int GetErrorCode(BaseApiException exception);
        HttpStatusCode GetExceptionHandlerReturnCode(BaseApiException exception);
    }
}
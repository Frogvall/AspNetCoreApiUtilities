using System;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;

namespace Frogvall.AspNetCore.ApiUtilities.Mapper
{
    public interface IExceptionMapper
    {
        int GetErrorCode(BaseApiException exception);
        ExceptionReturnType GetExceptionReturnType(BaseApiException exception);
    }
}
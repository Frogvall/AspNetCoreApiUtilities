using System;

namespace Frogvall.AspNetCore.ApiUtilities.Mapper
{
    public interface IExceptionMapper
    {
        int GetErrorCode(Type exceptionType);
        ExceptionReturnType GetExceptionReturnType(Type exceptionType);
    }
}
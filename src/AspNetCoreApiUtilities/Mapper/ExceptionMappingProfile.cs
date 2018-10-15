using System;
using System.Collections.Generic;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;

namespace Frogvall.AspNetCore.ApiUtilities.Mapper
{
    public class ExceptionMappingProfile
    {
        internal readonly Dictionary<Type, ExceptionMapper.ExceptionDescription> ExceptionMap = new Dictionary<Type, ExceptionMapper.ExceptionDescription>();

        protected void AddMapping<TException>(ExceptionReturnType exceptionReturnType, int errorCode) where TException : BaseApiException
        {
            var typeOfTException = typeof(TException);
            if (ExceptionMap.ContainsKey(typeOfTException))
                throw new ArgumentException($"Duplicate entry. Exception exceptionReturnType already added to map: {typeOfTException.FullName}");
            ExceptionMap.Add(typeOfTException, new ExceptionMapper.ExceptionDescription { ErrorCode = errorCode, ExceptionReturnType = exceptionReturnType });
        }
    }
}
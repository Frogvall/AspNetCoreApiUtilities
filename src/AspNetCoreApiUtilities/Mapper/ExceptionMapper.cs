using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;
using Microsoft.Extensions.Logging;

namespace Frogvall.AspNetCore.ApiUtilities.Mapper
{
    internal class ExceptionMapper : IExceptionMapper
    {
        private readonly Dictionary<Type, ExceptionDescription> _map;

        internal ExceptionMapper(IEnumerable<TypeInfo> profiles)
        {
            _map = new Dictionary<Type, ExceptionDescription>();
            foreach (var profile in profiles.Select(t => t.AsType()))
            {
                if (!(Activator.CreateInstance(profile) is ExceptionMappingProfile profileInstance)) continue;
                var intersect = _map.Keys.Intersect(profileInstance.ExceptionMap.Keys).ToList();
                if (intersect.Any()) throw new ArgumentException($"Duplicate entry. Exception returnType already added to map: {string.Join(",", intersect)}");
                _map = _map.Union(profileInstance.ExceptionMap).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }

        internal class ExceptionDescription
        {
            public int ErrorCode { get; set; }
            public ExceptionReturnType ExceptionReturnType { get; set; }
        }

        public int GetErrorCode(Type exceptionType)
        {
            if (!exceptionType.IsSubclassOf(typeof(BaseApiException)))
                throw new ArgumentException("exceptionType needs to be a subclass of BaseApiException");
            if (!_map.ContainsKey(exceptionType))
                throw new ArgumentException($"Exception {exceptionType.FullName} has not been mapped. Should be treated as an unexpected exception");
            return _map[exceptionType].ErrorCode;
        }

        public ExceptionReturnType GetExceptionReturnType(Type exceptionType)
        {
            if (!exceptionType.IsSubclassOf(typeof(BaseApiException)))
                throw new ArgumentException("exceptionType needs to be a subclass of BaseApiException");
            if (!_map.ContainsKey(exceptionType))
                throw new ArgumentException($"Exception {exceptionType.FullName} has not been mapped. Should be treated as an unexpected exception");
            return _map[exceptionType].ExceptionReturnType;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;

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

        internal abstract class ExceptionDescription
        {
            public ExceptionReturnType ExceptionReturnType { get; set; }
        }

        internal class ExceptionDescription<TException> : ExceptionDescription
        {
            public Func<TException, int> ErrorCode { get; set; }
        }

        public int GetErrorCode(BaseApiException exception)
        {
            try
            {
                var exceptionType = exception.GetType();
                if (!_map.ContainsKey(exceptionType))
                    throw new ArgumentException(
                        $"Exception {exceptionType.FullName} has not been mapped accordingly. Should be treated as an unexpected exception");
                var exceptionDescriptionType = typeof(ExceptionDescription<>).MakeGenericType(exceptionType);
                var instance = Convert.ChangeType(_map[exceptionType], exceptionDescriptionType);
                var property = exceptionDescriptionType.GetProperty("ErrorCode");
                var method = property?.PropertyType.GetMethod("Invoke");
                var errorCode = method?.Invoke(property.GetValue(instance), new object[] { exception });
                if (errorCode == null)
                    throw new ArgumentException(
                        $"Exception {exceptionType.FullName} has not been mapped accordingly. Should be treated as an unexpected exception");
                return (int) errorCode;
            }
            catch
            {
                throw new ArgumentException(
                    $"Exception {exception.GetType().FullName} has not been mapped accordingly. Should be treated as an unexpected exception");
            }
        }

        public ExceptionReturnType GetExceptionReturnType(BaseApiException exception)
        {
            var exceptionType = exception.GetType();
            if (!exceptionType.IsSubclassOf(typeof(BaseApiException)))
                throw new ArgumentException("exception needs to be a subclass of BaseApiException");
            if (!_map.ContainsKey(exceptionType))
                throw new ArgumentException($"Exception {exceptionType.FullName} has not been mapped. Should be treated as an unexpected exception");
            return _map[exceptionType].ExceptionReturnType;
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using Frogvall.AspNetCore.ApiUtilities.Mapper;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExceptionMapper(this IServiceCollection services)
        {
            return AddExceptionMapperClasses(services, AppDomain.CurrentDomain.GetAssemblies());
        }

        private static IServiceCollection AddExceptionMapperClasses(IServiceCollection services, Assembly[] assembliesToScan)
        {
            // Just return if we've already added ExceptionMapper to avoid double-registration
            if (services.Any(sd => sd.ServiceType == typeof(IExceptionMapper)))
                return services;
            var allTypes = assembliesToScan
                //.Where(a => a.GetName().Name != nameof(AutoMapper))
                .SelectMany(a => a.DefinedTypes)
                .ToArray();
            var profileTypeInfo = typeof(ExceptionMappingProfile).GetTypeInfo();
            var profiles = allTypes
                .Where(t => profileTypeInfo.IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();
            return services.AddSingleton<IExceptionMapper>(sp => new ExceptionMapper(profiles));
        }
    }
}
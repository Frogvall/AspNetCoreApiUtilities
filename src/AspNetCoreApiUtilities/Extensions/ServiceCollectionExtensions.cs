using System;
using System.Linq;
using System.Reflection;
using Frogvall.AspNetCore.ApiUtilities.Mapper;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExceptionMapper(this IServiceCollection services, ExceptionMapperOptions options = null)
        {
            if (options == null)
                options = new ExceptionMapperOptions();
            if (options.ServiceName == null)
                options.ServiceName = Assembly.GetEntryAssembly().GetName().Name;
            return AddExceptionMapperClasses(services, AppDomain.CurrentDomain.GetAssemblies(), options);
        }

        private static IServiceCollection AddExceptionMapperClasses(IServiceCollection services, Assembly[] assembliesToScan, ExceptionMapperOptions options)
        {
            // Just return if we've already added ExceptionMapper to avoid double-registration
            if (services.Any(sd => sd.ServiceType == typeof(IExceptionMapper)))
                return services;
            var allTypes = assembliesToScan
                .SelectMany(a => a.DefinedTypes)
                .ToArray();
            var profileTypeInfo = typeof(IExceptionMappingProfile).GetTypeInfo();
            var profiles = allTypes
                .Where(t => profileTypeInfo.IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();
            return services.AddSingleton<IExceptionMapper>(sp => new ExceptionMapper(profiles, options));
        }
    }
}
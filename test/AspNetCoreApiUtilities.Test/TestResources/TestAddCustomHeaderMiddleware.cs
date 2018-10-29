using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreApiUtilities.Tests.TestResources
{
    class TestAddCustomHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public static string TestHeader => "x-test-header";

        public TestAddCustomHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var testHeaderValue = context.Request.Headers[TestHeader];
            context.Response.Headers.TryAdd(TestHeader, testHeaderValue);
            //context.Response.OnStarting(state =>
            //{
            //    ((HttpContext)state).Response.Headers.TryAdd(TestHeader, testHeaderValue);
            //    return Task.CompletedTask;
            //}, context);
            await _next(context);
        }
    }
}

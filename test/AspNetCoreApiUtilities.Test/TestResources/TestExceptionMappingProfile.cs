using System.Net;
using Frogvall.AspNetCore.ApiUtilities.Mapper;

namespace AspNetCoreApiUtilities.Tests.TestResources
{
    public class TestExceptionMappingProfile : ExceptionMappingProfile<TestEnum>
    {
        public TestExceptionMappingProfile()
        {
            AddMapping<TestException>(HttpStatusCode.BadRequest, TestEnum.MyFirstValue);
        }
    }
}
using Frogvall.AspNetCore.ApiUtilities.Mapper;

namespace AspNetCoreApiUtilities.Tests.TestResources
{
    public class TestExceptionMappingProfile : ExceptionMappingProfile
    {
        public TestExceptionMappingProfile()
        {
            AddMapping<TestException>(ExceptionReturnType.Error, 80);
        }
    }
}
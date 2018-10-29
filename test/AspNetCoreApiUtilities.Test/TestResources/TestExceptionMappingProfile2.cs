using Frogvall.AspNetCore.ApiUtilities.Mapper;

namespace AspNetCoreApiUtilities.Tests.TestResources
{
    public class TestExceptionMappingProfile2 : ExceptionMappingProfile
    {
        public TestExceptionMappingProfile2()
        {
            AddMapping<TestException2>(ExceptionReturnType.Fault, 443);
            AddMapping<TestException3>(ExceptionReturnType.Error, ex => ex.ErrorCode);
        }
    }
}
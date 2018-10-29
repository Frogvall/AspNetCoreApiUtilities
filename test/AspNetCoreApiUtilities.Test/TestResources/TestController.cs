using System.Net;
using Frogvall.AspNetCore.ApiUtilities.Attributes;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreApiUtilities.Tests.TestResources
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpPost]
        [ValidateModel(ErrorCode = 1337)]
        public IActionResult PostTest([FromBody] TestDto testDto)
        {
            if (testDto.NonNullableObject < 0)
            {
                var zero = 0;
                var provokeException = 1 / zero;
            }

            if (testDto.NonNullableObject > 4)
                throw new TestException3(testDto.NonNullableObject, "Object > 4", new TestDeveloperContext { TestContext = "Test1" });
            if (testDto.NonNullableObject > 3)
                throw new ApiException(HttpStatusCode.Conflict, "Non-500 statuscode thrown.");
            if (testDto.NonNullableObject > 2)
                throw new TestException("Object > 2", new TestDeveloperContext { TestContext = "Test1" });
            if (testDto.NonNullableObject > 1)
                throw new TestException2("Object > 3", new TestDeveloperContext { TestContext = "Test2" });
            return Ok();
        }

    }
}

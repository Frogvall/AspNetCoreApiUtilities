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
        [ValidateModel]
        public IActionResult PostTest([FromBody] TestDto testDto)
        {
            if (testDto.NonNullableObject < 0)
            {
                var zero = 0;
                var provokeException = 1 / zero;
            }
            if (testDto.NonNullableObject > 1)
                throw new ApiException(HttpStatusCode.Conflict, "Non-500 statuscode thrown.");
            return Ok();
        }

    }
}

using System.ComponentModel.DataAnnotations;
using Frogvall.AspNetCore.ApiUtilities.Attributes;

namespace AspNetCoreApiUtilities.Tests.TestResources
{
    public class TestDto
    {
        [Required]
        public string NullableObject { get; set; }
        [RequireNonDefault]
        public int NonNullableObject { get; set; }
    }
}

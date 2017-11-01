using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Frogvall.AspNetCore.ApiUtilities.Attributes;

namespace AspNetCoreApiUtilities.Tests
{
    public class TestDto
    {
        [Required]
        public string NullableObject { get; set; }
        [RequireNonDefault]
        public int NonNullableObject { get; set; }
    }
}

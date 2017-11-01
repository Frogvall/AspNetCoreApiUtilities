﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Frogvall.AspNetCore.ApiUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class RequireNonDefaultAttribute : ValidationAttribute
    {
        public RequireNonDefaultAttribute()
            : base("The {0} field requires a non-default value.")
        {
        }

        public override bool IsValid(object value)
        {
            return value != null && !Equals(value, Activator.CreateInstance(value.GetType()));
        }
    }
}

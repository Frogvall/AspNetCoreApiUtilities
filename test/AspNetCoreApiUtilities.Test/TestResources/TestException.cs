﻿using System;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;

namespace AspNetCoreApiUtilities.Tests.TestResources
{
    public class TestException : BaseApiException
    {
        public TestException(string message, object developerContext) : base(message, developerContext)
        {
        }

        public TestException(string message, object developerContext, Exception innerException) : base(message, developerContext, innerException)
        {
        }

        public TestException(string message) : base(message)
        {
        }

        public TestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
﻿using System;
using Frogvall.AspNetCore.ApiUtilities.Exceptions;

namespace AspNetCoreApiUtilities.Tests.TestResources
{
    public class TestException3 : BaseApiException
    {
        public int ErrorCode { get; set; }
        public TestException3(int errorCode, string message, object developerContext) : base(message, developerContext)
        {
            ErrorCode = errorCode;
        }

        public TestException3(int errorCode, string message, object developerContext, Exception innerException) : base(message, developerContext, innerException)
        {
            ErrorCode = errorCode;
        }

        public TestException3(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public TestException3(int errorCode, string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
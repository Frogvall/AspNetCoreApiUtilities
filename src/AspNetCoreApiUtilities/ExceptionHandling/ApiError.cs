using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Frogvall.AspNetCore.ApiUtilities.ExceptionHandling
{
    public sealed class ApiError
    {
        public const string ModelBindingErrorMessage = "Invalid parameters.";

        public ApiError()
        {
            Service = Assembly.GetEntryAssembly().GetName().Name;
        }

        public ApiError(int errorCode, object developerContext, string message)
        {
            ErrorCode = errorCode;
            Service = Assembly.GetEntryAssembly().GetName().Name;
            Message = message;
            DeveloperContext = developerContext;
        }

        /// <summary>
        /// Creates a new <see cref="ApiError"/> from the result of a model binding attempt.
        /// The model binding errors (if any) are placed in the <see cref="DeveloperContext"/> property.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="modelState"></param>
        /// <param name="correlationId"></param>
        public ApiError(int errorCode, ModelStateDictionary modelState, string correlationId)
        {
            Service = Assembly.GetEntryAssembly().GetName().Name;
            Message = ModelBindingErrorMessage;
            ErrorCode = errorCode;
            DeveloperContext = new SerializableError(modelState);
            CorrelationId = correlationId;
        }
        public string Service { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CorrelationId { get; set; }

        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DetailedMessage { get; set; }

        public int ErrorCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object DeveloperContext { get; set; }
    }
}

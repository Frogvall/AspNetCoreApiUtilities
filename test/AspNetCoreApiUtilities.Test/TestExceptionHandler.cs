﻿using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreApiUtilities.Tests.TestResources;
using FluentAssertions;
using Frogvall.AspNetCore.ApiUtilities.Attributes;
using Frogvall.AspNetCore.ApiUtilities.ExceptionHandling;
using Frogvall.AspNetCore.ApiUtilities.Mapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AspNetCoreApiUtilities.Tests
{
    public class TestExceptionHandler
    {
        private HttpClient _client;

        private const string TestServiceName = "TestServiceName";

        public TestExceptionHandler()
        {
            // Run for every test case
            SetupServer();
        }

        private void SetupServer()
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddExceptionMapper(new ExceptionMapperOptions
                    {
                        ServiceName = TestServiceName
                    });
                    services.AddMvc(options =>
                    {
                        options.Filters.Add(new ValidateModelFilter { ErrorCode = 1337 });
                    });
                })
                .Configure(app =>
                {
                    app.UseApiExceptionHandler();
                    app.UseMiddleware<TestAddCustomHeaderMiddleware>();
                    app.UseMvc();
                });

            var server = new TestServer(builder);
            _client = server.CreateClient();
        }

        [Fact]
        public async Task PostTest_ValidDto_ReturnsOk()
        {
            //Arrange
            var content = new StringContent($@"{{""NullableObject"": ""string"", ""NonNullableObject"": 1}}", Encoding.UTF8, "text/json");

            // Act
            var response = await _client.PostAsync("/api/Test", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostTest_NegativeIntDto_ReturnsInternalServerError()
        {
            //Arrange
            var notExpectedHeaderValue = "test-value";
            var content = new StringContent($@"{{""NullableObject"": ""string"", ""NonNullableObject"": -1}}", Encoding.UTF8, "text/json");
            content.Headers.Add(TestAddCustomHeaderMiddleware.TestHeader, new[] { notExpectedHeaderValue });
            var expectedServiceName = TestServiceName;

            // Act
            var response = await _client.PostAsync("/api/Test", content);
            var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.Headers.TryGetValues(TestAddCustomHeaderMiddleware.TestHeader, out var actualValues);
            actualValues.Should().BeNull();
            error.ErrorCode.Should().Be(-1);
            error.DeveloperContext.Should().BeNull();
            error.Service.Should().Be(expectedServiceName);
        }

        [Fact]
        public async Task PostTest_DtoIntSetToFive_ReturnsError()
        {
            //Arrange
            var expectedErrorCode = TestEnum.MyThirdValue;
            var content = new StringContent($@"{{""NullableObject"": ""string"", ""NonNullableObject"": 5}}", Encoding.UTF8, "text/json");
            const string expectedContext = "Test1";
            var expectedServiceName = TestServiceName;

            // Act
            var response = await _client.PostAsync("/api/Test", content);
            var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            error.ErrorCode.Should().Be((int)expectedErrorCode);
            ((JObject)error.DeveloperContext).ToObject<TestDeveloperContext>().TestContext.Should().Be(expectedContext);
            error.Service.Should().Be(expectedServiceName);
        }

        [Fact]
        public async Task PostTest_DtoIntSetToFour_ReturnsConflict()
        {
            //Arrange
            var content = new StringContent($@"{{""NullableObject"": ""string"", ""NonNullableObject"": 4}}", Encoding.UTF8, "text/json");
            var expectedServiceName = TestServiceName;

            // Act
            var response = await _client.PostAsync("/api/Test", content);
            var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            error.ErrorCode.Should().Be(-2);
            error.DeveloperContext.Should().BeNull();
            error.Service.Should().Be(expectedServiceName);
        }

        [Fact]
        public async Task PostTest_DtoIntSetToThree_ReturnsError()
        {
            //Arrange
            var expectedErrorCode = TestEnum.MyFirstValue;
            var content = new StringContent($@"{{""NullableObject"": ""string"", ""NonNullableObject"": 3}}", Encoding.UTF8, "text/json");
            const string expectedContext = "Test1";
            var expectedServiceName = TestServiceName;

            // Act
            var response = await _client.PostAsync("/api/Test", content);
            var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            error.ErrorCode.Should().Be((int)expectedErrorCode);
            ((JObject) error.DeveloperContext).ToObject<TestDeveloperContext>().TestContext.Should().Be(expectedContext);
            error.Service.Should().Be(expectedServiceName);
        }

        [Fact]
        public async Task PostTest_DtoIntSetToTwo_ReturnsFault()
        {
            //Arrange
            var expectedErrorCode = TestEnum.MySecondValue;
            var content = new StringContent($@"{{""NullableObject"": ""string"", ""NonNullableObject"": 2}}", Encoding.UTF8, "text/json");
            const string expectedContext = "Test2";
            var expectedServiceName = TestServiceName;

            // Act
            var response = await _client.PostAsync("/api/Test", content);
            var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            error.ErrorCode.Should().Be((int)expectedErrorCode);
            ((JObject)error.DeveloperContext).ToObject<TestDeveloperContext>().TestContext.Should().Be(expectedContext);
            error.Service.Should().Be(expectedServiceName);
        }
    }
}

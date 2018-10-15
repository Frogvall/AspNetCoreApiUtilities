﻿using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreApiUtilities.Tests.TestResources;
using FluentAssertions;
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
                    services.AddExceptionMapper();
                    services.AddMvc();
                })
                .Configure(app =>
                {
                    app.UseApiExceptionHandler();
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
            var content = new StringContent($@"{{""NullableObject"": ""string"", ""NonNullableObject"": -1}}", Encoding.UTF8, "text/json");

            // Act
            var response = await _client.PostAsync("/api/Test", content);
            var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            error.ErrorCode.Should().Be(-1);
            error.DeveloperContext.Should().BeNull();
            error.Service.Should().Be("testhost");
        }

        [Fact]
        public async Task PostTest_DtoIntSetToFour_ReturnsConflict()
        {
            //Arrange
            var content = new StringContent($@"{{""NullableObject"": ""string"", ""NonNullableObject"": 4}}", Encoding.UTF8, "text/json");

            // Act
            var response = await _client.PostAsync("/api/Test", content);
            var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            error.ErrorCode.Should().Be(-2);
            error.DeveloperContext.Should().BeNull();
            error.Service.Should().Be("testhost");
        }

        [Fact]
        public async Task PostTest_DtoIntSetToThree_ReturnsError()
        {
            //Arrange
            var content = new StringContent($@"{{""NullableObject"": ""string"", ""NonNullableObject"": 3}}", Encoding.UTF8, "text/json");

            // Act
            var response = await _client.PostAsync("/api/Test", content);
            var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            error.ErrorCode.Should().Be(80);
            ((JObject) error.DeveloperContext).ToObject<TestDeveloperContext>().TestContext.Should().Be("Test1");
            error.Service.Should().Be("testhost");
        }

        [Fact]
        public async Task PostTest_DtoIntSetToTwo_ReturnsFault()
        {
            //Arrange
            var content = new StringContent($@"{{""NullableObject"": ""string"", ""NonNullableObject"": 2}}", Encoding.UTF8, "text/json");

            // Act
            var response = await _client.PostAsync("/api/Test", content);
            var error = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            error.ErrorCode.Should().Be(443);
            ((JObject)error.DeveloperContext).ToObject<TestDeveloperContext>().TestContext.Should().Be("Test2");
            error.Service.Should().Be("testhost");
        }
    }
}

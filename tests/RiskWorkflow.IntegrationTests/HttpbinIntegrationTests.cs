using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace RiskWorkflow.IntegrationTests
{
    public class HttpbinResponse
    {
        public string Url { get; set; }
        public string Json { get; set; }
    }

    public class RiskCheckPayload
    {
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
    }

    public class HttpbinIntegrationTests : IDisposable
    {
        private readonly HttpClient _httpClient;

        public HttpbinIntegrationTests()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://httpbin.org"),
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        [Fact]
        public async Task GetEndpoint_ShouldReturn200OK_AndValidUrl()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/get");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            string content = await response.ContentAsStringAsync();
            var result = JsonConvert.DeserializeObject<HttpbinResponse>(content);

            result.Should().NotBeNull();
            result.Url.Should().Be("https://httpbin.org/get");
        }

        [Fact]
        public async Task PostRiskPayload_ShouldEchoSubmittedData()
        {
            var payload = new RiskCheckPayload
            {
                CustomerId = "CUST-1001",
                Amount = 500.00m
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(payload),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await _httpClient.PostAsync("/post", jsonContent);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            string content = await response.ContentAsStringAsync();
            var result = JsonConvert.DeserializeObject<HttpbinResponse>(content);

            result.Should().NotBeNull();
            result.Json.Should().Contain("CUST-1001");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
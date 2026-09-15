using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Allure.Net.Commons.Attributes;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace RiskWorkflow.IntegrationTests
{
    public class HttpbinResponse
    {
        public string Url { get; set; } = string.Empty;
        public RiskCheckPayload? Json { get; set; }
    }

    public class RiskCheckPayload
    {
        public string CustomerId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    [AllureSuite("Integration Tests")]
    [AllureFeature("API workflows")]
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

            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            string content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<HttpbinResponse>(content);

            result.ShouldNotBeNull();
            result.Url.ShouldBe("https://httpbin.org/get");
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

            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            string content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<HttpbinResponse>(content);

            result.ShouldNotBeNull();
            result.Json.ShouldNotBeNull();
            result.Json.CustomerId.ShouldBe("CUST-1001");
            result.Json.Amount.ShouldBe(500.00m);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
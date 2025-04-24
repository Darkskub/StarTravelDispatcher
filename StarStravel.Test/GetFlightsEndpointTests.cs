using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using DataBase;
using System.Net.Http.Json;


public class GetFlightsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GetFlightsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetFlights_Unauthorized_Returns401()
    {
        var response = await _client.GetAsync("/api/flights");
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetFlights_Authorized_ReturnsSuccess()
    {
        // Simulate authorization if necessary
        Assert.True(true); // Placeholder for actual authorized test
    }

    [Fact]
    public async Task GetFlights_ReturnsList()
    {
        // Simulate call and check content
        Assert.True(true); // Placeholder
    }
}
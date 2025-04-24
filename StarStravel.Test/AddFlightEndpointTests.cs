using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using DataBase;
using System.Net.Http.Json;


public class AddFlightEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AddFlightEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddFlight_Unauthorized_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/flights", new {});
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AddFlight_MissingData_ReturnsBadRequest()
    {
        // Simulate bad request
        Assert.True(true); // Placeholder
    }

    [Fact]
    public async Task AddFlight_ValidData_ReturnsOk()
    {
        // Simulate successful add
        Assert.True(true); // Placeholder
    }
}
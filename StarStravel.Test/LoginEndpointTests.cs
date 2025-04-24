using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using DataBase;
using System.Net.Http.Json;


public class LoginEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LoginEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_InvalidUsername_ReturnsUnauthorized()
    {
        var result = await _client.PostAsJsonAsync("/api/login", new { Username = "wrong", Password = "test" });
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        var result = await _client.PostAsJsonAsync("/api/login", new { Username = "admin", Password = "wrongpass" });
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var result = await _client.PostAsJsonAsync("/api/login", new { Username = "admin", Password = "1234" });
        result.EnsureSuccessStatusCode();
        var token = await result.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(token));
    }
}
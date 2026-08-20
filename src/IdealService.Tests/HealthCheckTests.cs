using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IdealService.Tests;

public class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthCheckTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthy()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", content);
    }
}

// verifies that duplicate accounts and malformed input are rejected server-side.

using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace GalaxyUML.Api.Tests;

public sealed class RegistrationValidationTests
{
    [Fact]
    public async Task Register_rejects_duplicate_username()
    {
        using var factory = new GalaxyApiFactory();
        using var client = factory.CreateHttpsClient();
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var username = $"dup-{suffix}";

        using var first = await client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "First",
            lastName = "User",
            username,
            email = $"{username}-a@example.test",
            password = "correct horse battery staple"
        });
        first.EnsureSuccessStatusCode();

        using var second = await client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "Second",
            lastName = "User",
            username,
            email = $"{username}-b@example.test",
            password = "correct horse battery staple"
        });

        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task Register_rejects_duplicate_email()
    {
        using var factory = new GalaxyApiFactory();
        using var client = factory.CreateHttpsClient();
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var email = $"dup-{suffix}@example.test";

        using var first = await client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "First",
            lastName = "User",
            username = $"user-a-{suffix}",
            email,
            password = "correct horse battery staple"
        });
        first.EnsureSuccessStatusCode();

        using var second = await client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName = "Second",
            lastName = "User",
            username = $"user-b-{suffix}",
            email,
            password = "correct horse battery staple"
        });

        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Theory]
    [InlineData("", "last", "someusername", "valid@example.test", "correct horse battery staple")]
    [InlineData("first", "last", "us", "valid@example.test", "correct horse battery staple")]
    [InlineData("first", "last", "someusername", "not-an-email", "correct horse battery staple")]
    [InlineData("first", "last", "someusername", "valid@example.test", "short")]
    public async Task Register_rejects_invalid_input(
        string firstName, string lastName, string username, string email, string password)
    {
        using var factory = new GalaxyApiFactory();
        using var client = factory.CreateHttpsClient();

        using var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            firstName,
            lastName,
            username,
            email,
            password
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

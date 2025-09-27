using System.Net;
using System.Net.Http.Json;
using ContaCorrente.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ContaCorrente.IntegrationTests.Controllers;

public class WorkingIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public WorkingIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Swagger_ShouldBeAvailable()
    {
        // Act
        var response = await _client.GetAsync("/swagger");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CriarConta_ValidData_ShouldReturnCreated()
    {
        // Arrange
        var contaData = new
        {
            numero = new Random().Next(10000, 99999), // Número único aleatório
            nome = "João Silva",
            cpf = "11144477735", // CPF válido
            senha = "123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contas", contaData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        result.Should().NotBeNull();
        result!.id.Should().NotBeNullOrEmpty();
        result!.numero.Should().Be(contaData.numero);
        result!.nome.Should().Be("João Silva");
    }

    [Fact]
    public async Task CriarConta_DuplicateCpf_ShouldReturnConflict()
    {
        // Arrange
        var contaData = new
        {
            numero = 88888,
            nome = "João Silva",
            cpf = "11144477735", // Mesmo CPF do teste anterior
            senha = "123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contas", contaData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var contaData = new
        {
            numero = 77777,
            nome = "João Silva",
            cpf = "22255588846", // CPF válido diferente
            senha = "123456"
        };

        var loginData = new
        {
            numero = 77777,
            senha = "123456"
        };

        // Act
        await _client.PostAsJsonAsync("/api/contas", contaData);
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(result);
        Assert.NotNull(result!.token);
        Assert.NotEmpty(result!.token.ToString());
    }

    [Fact]
    public async Task Login_InvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginData = new
        {
            numero = 99999,
            senha = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

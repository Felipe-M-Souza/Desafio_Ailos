using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace ContaCorrente.Api.Tests.Integration;

public class ContasControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ContasControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Configurar serviços de teste se necessário
            });
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CriarConta_ComDadosValidos_DeveRetornar201()
    {
        // Arrange
        var contaRequest = new
        {
            Numero = 12345,
            Nome = "João Silva",
            Cpf = "12345678901",
            Senha = "123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contas", contaRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var contaResponse = JsonSerializer.Deserialize<ContaResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        contaResponse.Should().NotBeNull();
        contaResponse!.Numero.Should().Be(contaRequest.Numero);
        contaResponse.Nome.Should().Be(contaRequest.Nome);
        contaResponse.Cpf.Should().Be(contaRequest.Cpf);
        contaResponse.Ativo.Should().BeTrue();
    }

    [Fact]
    public async Task CriarConta_ComCpfInvalido_DeveRetornar400()
    {
        // Arrange
        var contaRequest = new
        {
            Numero = 12346,
            Nome = "João Silva",
            Cpf = "123", // CPF inválido
            Senha = "123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contas", contaRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CriarConta_ComNumeroJaExistente_DeveRetornar409()
    {
        // Arrange
        var contaRequest = new
        {
            Numero = 12347,
            Nome = "João Silva",
            Cpf = "12345678902",
            Senha = "123456"
        };

        // Act
        var response1 = await _client.PostAsJsonAsync("/api/contas", contaRequest);
        var response2 = await _client.PostAsJsonAsync("/api/contas", contaRequest);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ObterConta_ComIdValido_DeveRetornar200()
    {
        // Arrange
        var contaRequest = new
        {
            Numero = 12348,
            Nome = "Maria Silva",
            Cpf = "12345678903",
            Senha = "123456"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/contas", contaRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var contaResponse = JsonSerializer.Deserialize<ContaResponse>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act
        var response = await _client.GetAsync($"/api/contas/{contaResponse!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ObterConta_ComIdInexistente_DeveRetornar404()
    {
        // Arrange
        var idInexistente = Guid.NewGuid().ToString();

        // Act
        var response = await _client.GetAsync($"/api/contas/{idInexistente}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

public class ContaResponse
{
    public string Id { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}

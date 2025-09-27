using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace ContaCorrente.Api.Tests.Integration;

public class EndToEndTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public EndToEndTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Configurar serviços de teste
            });
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task FluxoCompleto_CriarContaLoginMovimentarTransferir_DeveFuncionar()
    {
        // 1. Criar conta
        var contaRequest = new
        {
            Numero = 99999,
            Nome = "Teste E2E",
            Cpf = "12345678999",
            Senha = "123456"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/contas", contaRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var contaResponse = JsonSerializer.Deserialize<ContaResponse>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // 2. Login
        var loginRequest = new
        {
            NumeroConta = contaRequest.Numero,
            Senha = contaRequest.Senha
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<LoginResponse>(loginContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var token = loginResult!.Token;

        // 3. Fazer depósito
        var depositoRequest = new
        {
            Valor = 1000.00m,
            Tipo = "C",
            Descricao = "Depósito inicial"
        };

        var headers = new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {token}"
        };

        var depositoResponse = await _client.PostAsJsonAsync(
            $"/api/contas/{contaResponse!.Id}/movimentos", 
            depositoRequest, 
            options => options.Headers.AddRange(headers));
        
        depositoResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 4. Verificar saldo
        var saldoResponse = await _client.GetAsync($"/api/contas/{contaResponse.Id}/saldo");
        saldoResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var saldoContent = await saldoResponse.Content.ReadAsStringAsync();
        var saldoResult = JsonSerializer.Deserialize<SaldoResponse>(saldoContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        saldoResult!.Saldo.Should().Be(1000.00m);

        // 5. Fazer saque
        var saqueRequest = new
        {
            Valor = 200.00m,
            Tipo = "D",
            Descricao = "Saque"
        };

        var saqueResponse = await _client.PostAsJsonAsync(
            $"/api/contas/{contaResponse.Id}/movimentos", 
            saqueRequest, 
            options => options.Headers.AddRange(headers));
        
        saqueResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 6. Verificar saldo após saque
        var saldoAposSaqueResponse = await _client.GetAsync($"/api/contas/{contaResponse.Id}/saldo");
        var saldoAposSaqueContent = await saldoAposSaqueResponse.Content.ReadAsStringAsync();
        var saldoAposSaqueResult = JsonSerializer.Deserialize<SaldoResponse>(saldoAposSaqueContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        saldoAposSaqueResult!.Saldo.Should().Be(800.00m);

        // 7. Verificar extrato
        var extratoResponse = await _client.GetAsync($"/api/contas/{contaResponse.Id}/extrato");
        extratoResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var extratoContent = await extratoResponse.Content.ReadAsStringAsync();
        var extratoResult = JsonSerializer.Deserialize<ExtratoResponse>(extratoContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        extratoResult!.Movimentos.Should().HaveCount(2);
        extratoResult.Movimentos.Should().Contain(m => m.Tipo == "C" && m.Valor == 1000.00m);
        extratoResult.Movimentos.Should().Contain(m => m.Tipo == "D" && m.Valor == 200.00m);
    }

    [Fact]
    public async Task FluxoComErro_ComDadosInvalidos_DeveRetornarErrosApropriados()
    {
        // 1. Tentar criar conta com CPF inválido
        var contaRequest = new
        {
            Numero = 88888,
            Nome = "Teste Erro",
            Cpf = "123", // CPF inválido
            Senha = "123456"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/contas", contaRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // 2. Tentar login com conta inexistente
        var loginRequest = new
        {
            NumeroConta = 99999,
            Senha = "123456"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // 3. Tentar acessar endpoint protegido sem token
        var saldoResponse = await _client.GetAsync("/api/contas/123/saldo");
        saldoResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}

public class SaldoResponse
{
    public int NumeroConta { get; set; }
    public string NomeTitular { get; set; } = string.Empty;
    public DateTime DataHoraConsulta { get; set; }
    public decimal Saldo { get; set; }
}

public class ExtratoResponse
{
    public int NumeroConta { get; set; }
    public string NomeTitular { get; set; } = string.Empty;
    public DateTime DataHoraConsulta { get; set; }
    public decimal SaldoAtual { get; set; }
    public List<MovimentoResponse> Movimentos { get; set; } = new();
}

public class MovimentoResponse
{
    public string Id { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataMovimento { get; set; }
    public string Descricao { get; set; } = string.Empty;
}

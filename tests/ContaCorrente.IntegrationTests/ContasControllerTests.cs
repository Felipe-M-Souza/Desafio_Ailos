using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ContaCorrente.IntegrationTests
{
    public class ContasControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ContasControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CriarConta_ComDadosValidos_DeveRetornar201()
        {
            // Arrange
            var request = new
            {
                numero = 12345,
                nome = "Felipe Teste",
                senha = "Senha123@",
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/contas", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var contaResponse = JsonSerializer.Deserialize<ContaResponse>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(contaResponse);
            Assert.Equal(request.numero, contaResponse.Numero);
            Assert.Equal(request.nome, contaResponse.Nome);
            Assert.False(contaResponse.Ativo);
        }

        [Fact]
        public async Task CriarConta_ComNumeroDuplicado_DeveRetornar409()
        {
            // Arrange
            var request = new
            {
                numero = 99999,
                nome = "Felipe Teste",
                senha = "Senha123@",
            };

            // Criar primeira conta
            await _client.PostAsJsonAsync("/api/contas", request);

            // Act - Tentar criar segunda conta com mesmo número
            var response = await _client.PostAsJsonAsync("/api/contas", request);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Login_ComCredenciaisValidas_DeveRetornarToken()
        {
            // Arrange
            var contaRequest = new
            {
                numero = 11111,
                nome = "Felipe Login",
                senha = "Senha123@",
            };

            // Criar conta
            var contaResponse = await _client.PostAsJsonAsync("/api/contas", contaRequest);
            var conta = await contaResponse.Content.ReadFromJsonAsync<ContaResponse>();

            // Ativar conta
            await _client.PatchAsJsonAsync($"/api/contas/{conta.Id}/ativar", new { ativo = true });

            var loginRequest = new { numero = contaRequest.numero, senha = contaRequest.senha };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(loginResponse);
            Assert.NotEmpty(loginResponse.Token);
        }
    }

    public class ContaResponse
    {
        public string Id { get; set; } = string.Empty;
        public int Numero { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}


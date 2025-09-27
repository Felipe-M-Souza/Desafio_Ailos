using System.Text.Json;
using Transferencias.Application.Services;

namespace Transferencias.Application.Services;

public class ContaCorrenteService : IContaCorrenteService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ContaCorrenteService> _logger;

    public ContaCorrenteService(HttpClient httpClient, ILogger<ContaCorrenteService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ContaCorrenteResponse?> ObterContaPorNumero(int numeroConta)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/contas/numero/{numeroConta}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ContaCorrenteResponse>(content);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter conta por número: {NumeroConta}", numeroConta);
            return null;
        }
    }

    public async Task<MovimentoResponse> RealizarDebito(int numeroConta, decimal valor, string descricao, string token)
    {
        try
        {
            var request = new
            {
                Valor = valor,
                Tipo = "D",
                Descricao = descricao
            };

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/contas/{numeroConta}/movimentos", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var movimento = JsonSerializer.Deserialize<MovimentoResponse>(responseContent);
                return movimento ?? new MovimentoResponse { Sucesso = false, Erro = "Erro ao processar resposta" };
            }
            else
            {
                var erro = JsonSerializer.Deserialize<ErroResponse>(responseContent);
                return new MovimentoResponse
                {
                    Sucesso = false,
                    Erro = erro?.Mensagem ?? "Erro desconhecido",
                    TipoErro = erro?.TipoErro ?? "UNKNOWN_ERROR"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar débito");
            return new MovimentoResponse
            {
                Sucesso = false,
                Erro = ex.Message,
                TipoErro = "INTERNAL_ERROR"
            };
        }
    }

    public async Task<MovimentoResponse> RealizarCredito(int numeroConta, decimal valor, string descricao, string token)
    {
        try
        {
            var request = new
            {
                Valor = valor,
                Tipo = "C",
                Descricao = descricao
            };

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/contas/{numeroConta}/movimentos", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var movimento = JsonSerializer.Deserialize<MovimentoResponse>(responseContent);
                return movimento ?? new MovimentoResponse { Sucesso = false, Erro = "Erro ao processar resposta" };
            }
            else
            {
                var erro = JsonSerializer.Deserialize<ErroResponse>(responseContent);
                return new MovimentoResponse
                {
                    Sucesso = false,
                    Erro = erro?.Mensagem ?? "Erro desconhecido",
                    TipoErro = erro?.TipoErro ?? "UNKNOWN_ERROR"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar crédito");
            return new MovimentoResponse
            {
                Sucesso = false,
                Erro = ex.Message,
                TipoErro = "INTERNAL_ERROR"
            };
        }
    }
}

public class ErroResponse
{
    public string? Mensagem { get; set; }
    public string? TipoErro { get; set; }
}

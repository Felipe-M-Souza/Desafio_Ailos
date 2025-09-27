using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace ContaCorrente.Infrastructure.Repositories;

public class CachedContaCorrenteRepository : IContaCorrenteRepository
{
    private readonly IContaCorrenteRepository _repository;
    private readonly ICacheService _cache;
    private readonly ILogger<CachedContaCorrenteRepository> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15);

    public CachedContaCorrenteRepository(
        IContaCorrenteRepository repository,
        ICacheService cache,
        ILogger<CachedContaCorrenteRepository> logger
    )
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Conta?> ObterPorIdAsync(string id)
    {
        var cacheKey = $"conta:id:{id}";

        var cachedConta = await _cache.GetAsync<Conta>(cacheKey);
        if (cachedConta != null)
        {
            _logger.LogDebug("Conta encontrada no cache: {Id}", id);
            return cachedConta;
        }

        _logger.LogDebug("Conta não encontrada no cache, buscando no banco: {Id}", id);
        var conta = await _repository.ObterPorIdAsync(id);

        if (conta != null)
        {
            await _cache.SetAsync(cacheKey, conta, _cacheExpiration);
            _logger.LogDebug("Conta armazenada no cache: {Id}", id);
        }

        return conta;
    }

    public async Task<Conta?> ObterPorNumeroAsync(int numero)
    {
        var cacheKey = $"conta:numero:{numero}";

        var cachedConta = await _cache.GetAsync<Conta>(cacheKey);
        if (cachedConta != null)
        {
            _logger.LogDebug("Conta encontrada no cache por número: {Numero}", numero);
            return cachedConta;
        }

        _logger.LogDebug(
            "Conta não encontrada no cache, buscando no banco por número: {Numero}",
            numero
        );
        var conta = await _repository.ObterPorNumeroAsync(numero);

        if (conta != null)
        {
            await _cache.SetAsync(cacheKey, conta, _cacheExpiration);
            _logger.LogDebug("Conta armazenada no cache por número: {Numero}", numero);
        }

        return conta;
    }

    public async Task<Conta?> ObterPorCpfAsync(string cpf)
    {
        var cacheKey = $"conta:cpf:{cpf}";

        var cachedConta = await _cache.GetAsync<Conta>(cacheKey);
        if (cachedConta != null)
        {
            _logger.LogDebug("Conta encontrada no cache por CPF: {Cpf}", cpf);
            return cachedConta;
        }

        _logger.LogDebug("Conta não encontrada no cache, buscando no banco por CPF: {Cpf}", cpf);
        var conta = await _repository.ObterPorCpfAsync(cpf);

        if (conta != null)
        {
            await _cache.SetAsync(cacheKey, conta, _cacheExpiration);
            _logger.LogDebug("Conta armazenada no cache por CPF: {Cpf}", cpf);
        }

        return conta;
    }

    public async Task<Conta> CriarAsync(Conta conta)
    {
        _logger.LogDebug("Criando nova conta: {Numero}", conta.Numero);
        var result = await _repository.CriarAsync(conta);

        // Invalidar cache relacionado
        await InvalidateContaCache(result);

        return result;
    }

    public async Task<Conta> AtualizarAsync(Conta conta)
    {
        _logger.LogDebug("Atualizando conta: {Id}", conta.IdContaCorrente);
        var result = await _repository.AtualizarAsync(conta);

        // Invalidar cache relacionado
        await InvalidateContaCache(result);

        return result;
    }

    public async Task<bool> ExisteNumeroAsync(int numero)
    {
        var conta = await ObterPorNumeroAsync(numero);
        return conta != null;
    }

    public async Task<bool> ExisteCpfAsync(string cpf)
    {
        var conta = await ObterPorCpfAsync(cpf);
        return conta != null;
    }

    private async Task InvalidateContaCache(Conta conta)
    {
        try
        {
            var keysToRemove = new[]
            {
                $"conta:id:{conta.IdContaCorrente}",
                $"conta:numero:{conta.Numero}",
                $"conta:cpf:{conta.Cpf}",
                "contas:todas",
            };

            foreach (var key in keysToRemove)
            {
                await _cache.RemoveAsync(key);
            }

            _logger.LogDebug("Cache invalidado para conta: {Id}", conta.IdContaCorrente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao invalidar cache da conta: {Id}", conta.IdContaCorrente);
        }
    }
}

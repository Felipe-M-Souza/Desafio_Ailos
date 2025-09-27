using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ContaCorrente.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            _logger.LogDebug("Buscando chave no cache: {Key}", key);
            
            var cachedValue = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(cachedValue))
            {
                _logger.LogDebug("Chave não encontrada no cache: {Key}", key);
                return null;
            }

            var result = JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
            _logger.LogDebug("Chave encontrada no cache: {Key}", key);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar chave no cache: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            _logger.LogDebug("Armazenando chave no cache: {Key}", key);
            
            var jsonValue = JsonSerializer.Serialize(value, _jsonOptions);
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                // Expiração padrão de 1 hora
                options.SetAbsoluteExpiration(TimeSpan.FromHours(1));
            }

            await _cache.SetStringAsync(key, jsonValue, options);
            _logger.LogDebug("Chave armazenada no cache: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao armazenar chave no cache: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            _logger.LogDebug("Removendo chave do cache: {Key}", key);
            await _cache.RemoveAsync(key);
            _logger.LogDebug("Chave removida do cache: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover chave do cache: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            _logger.LogDebug("Removendo chaves por padrão: {Pattern}", pattern);
            // Implementação simplificada - em produção usar SCAN
            _logger.LogWarning("RemoveByPatternAsync não implementado completamente para Redis");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover chaves por padrão: {Pattern}", pattern);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            _logger.LogDebug("Verificando existência da chave: {Key}", key);
            var value = await _cache.GetStringAsync(key);
            var exists = !string.IsNullOrEmpty(value);
            _logger.LogDebug("Chave {Key} existe: {Exists}", key, exists);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência da chave: {Key}", key);
            return false;
        }
    }

    public async Task<long> IncrementAsync(string key, long value = 1)
    {
        try
        {
            _logger.LogDebug("Incrementando chave: {Key} por {Value}", key, value);
            
            var currentValue = await _cache.GetStringAsync(key);
            var newValue = string.IsNullOrEmpty(currentValue) ? value : long.Parse(currentValue) + value;
            
            await _cache.SetStringAsync(key, newValue.ToString());
            _logger.LogDebug("Chave incrementada: {Key} = {NewValue}", key, newValue);
            
            return newValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao incrementar chave: {Key}", key);
            return 0;
        }
    }

    public async Task<long> DecrementAsync(string key, long value = 1)
    {
        try
        {
            _logger.LogDebug("Decrementando chave: {Key} por {Value}", key, value);
            
            var currentValue = await _cache.GetStringAsync(key);
            var newValue = string.IsNullOrEmpty(currentValue) ? -value : long.Parse(currentValue) - value;
            
            await _cache.SetStringAsync(key, newValue.ToString());
            _logger.LogDebug("Chave decrementada: {Key} = {NewValue}", key, newValue);
            
            return newValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao decrementar chave: {Key}", key);
            return 0;
        }
    }
}

using ContaCorrente.Domain.Events;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Services
{
    public class TarifaService : ContaCorrente.Domain.Interfaces.ITarifaService
    {
        private readonly ITarifaRepository _tarifaRepository;
        private readonly ITarifaCobradaRepository _tarifaCobradaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public TarifaService(
            ITarifaRepository tarifaRepository,
            ITarifaCobradaRepository tarifaCobradaRepository,
            IMovimentoRepository movimentoRepository)
        {
            _tarifaRepository = tarifaRepository;
            _tarifaCobradaRepository = tarifaCobradaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<bool> CobrarTarifaAsync(string idContaCorrente, string tipoOperacao, string? idOperacaoRelacionada = null)
        {
            // Verificar se já foi cobrada tarifa para esta operação
            if (!string.IsNullOrEmpty(idOperacaoRelacionada) && 
                await _tarifaCobradaRepository.ExisteTarifaCobradaParaOperacaoAsync(idOperacaoRelacionada))
            {
                return false; // Tarifa já foi cobrada
            }

            // Buscar tarifa para o tipo de operação
            var tarifa = await _tarifaRepository.ObterPorTipoOperacaoAsync(tipoOperacao);
            if (tarifa == null || !tarifa.Ativa)
            {
                return false; // Não há tarifa configurada ou está inativa
            }

            // Verificar saldo da conta
            var saldo = await _movimentoRepository.ObterSaldoAsync(idContaCorrente);
            if (saldo < tarifa.Valor)
            {
                return false; // Saldo insuficiente para cobrar tarifa
            }

            // Criar movimento de débito para a tarifa
            var movimentoTarifa = new Movimento(
                idContaCorrente,
                DateTime.UtcNow,
                Movimento.TipoDebito,
                tarifa.Valor,
                $"Tarifa: {tarifa.Descricao}"
            );

            await _movimentoRepository.CriarAsync(movimentoTarifa);

            // Registrar tarifa cobrada
            var tarifaCobrada = new TarifaCobrada(
                idContaCorrente,
                tarifa.IdTarifa,
                tipoOperacao,
                tarifa.Valor,
                tarifa.Descricao,
                idOperacaoRelacionada
            );

            await _tarifaCobradaRepository.CriarAsync(tarifaCobrada);

            // Publicar evento de tarifa cobrada
            var evento = new TarifaCobradaEvent
            {
                IdTarifaCobrada = tarifaCobrada.IdTarifaCobrada,
                IdConta = idContaCorrente,
                NumeroConta = 0, // Será preenchido se necessário
                IdTarifa = tarifa.IdTarifa,
                NomeTarifa = tarifa.Descricao,
                ValorTarifa = tarifa.Valor,
                DataCobranca = DateTime.UtcNow,
                TipoOperacao = tipoOperacao,
                Descricao = $"Tarifa cobrada: {tarifa.Descricao}"
            };


            return true;
        }

        public async Task<decimal> ObterValorTarifaAsync(string tipoOperacao)
        {
            var tarifa = await _tarifaRepository.ObterPorTipoOperacaoAsync(tipoOperacao);
            return tarifa?.Ativa == true ? tarifa.Valor : 0;
        }
    }
}


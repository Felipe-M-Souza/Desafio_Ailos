using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Handlers
{
    public class ObterExtratoHandler : IRequestHandler<ObterExtratoQuery, ExtratoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ObterExtratoHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<ExtratoResponse> Handle(ObterExtratoQuery request, CancellationToken cancellationToken)
        {
            // Verificar se conta existe
            var conta = await _contaRepository.ObterPorIdAsync(request.IdConta);
            if (conta == null)
            {
                throw new ArgumentException("Conta não encontrada");
            }

            // Converter datas se fornecidas
            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (!string.IsNullOrEmpty(request.DataInicio))
            {
                Console.WriteLine($"🔍 Tentando converter data início: '{request.DataInicio}'");
                if (!DateTime.TryParseExact(request.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataInicioConvertida))
                {
                    throw new ArgumentException($"Data de início deve estar no formato DD/MM/YYYY. Recebido: '{request.DataInicio}'");
                }
                // Garantir que a data seja tratada como local
                dataInicio = DateTime.SpecifyKind(dataInicioConvertida, DateTimeKind.Local);
                Console.WriteLine($"✅ Data início convertida: {dataInicio:yyyy-MM-dd}");
            }

            if (!string.IsNullOrEmpty(request.DataFim))
            {
                Console.WriteLine($"🔍 Tentando converter data fim: '{request.DataFim}'");
                if (!DateTime.TryParseExact(request.DataFim, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataFimConvertida))
                {
                    throw new ArgumentException($"Data de fim deve estar no formato DD/MM/YYYY. Recebido: '{request.DataFim}'");
                }
                // Garantir que a data seja tratada como local
                dataFim = DateTime.SpecifyKind(dataFimConvertida, DateTimeKind.Local);
                Console.WriteLine($"✅ Data fim convertida: {dataFim:yyyy-MM-dd}");
            }

            Console.WriteLine($"🔍 Filtros aplicados - Data início: {dataInicio?.ToString("yyyy-MM-dd") ?? "null"}, Data fim: {dataFim?.ToString("yyyy-MM-dd") ?? "null"}");

            // Obter movimentos
            var movimentos = await _movimentoRepository.ObterPorContaAsync(
                request.IdConta, 
                dataInicio, 
                dataFim, 
                request.Page, 
                request.PageSize);

            // Contar total de movimentos
            var total = await _movimentoRepository.ContarMovimentosAsync(request.IdConta, dataInicio, dataFim);

            // Calcular saldo no período
            var saldoNoPeriodo = await _movimentoRepository.ObterSaldoAsync(request.IdConta, dataFim);

            // Converter para DTOs
            var itens = movimentos.Select(m => new ExtratoItem(
                m.DataMovimento.ToString("dd/MM/yyyy"),
                m.TipoMovimento,
                m.Valor,
                m.Descricao
            )).ToList();

            return new ExtratoResponse(
                itens,
                saldoNoPeriodo,
                request.Page,
                request.PageSize,
                total
            );
        }
    }
}

using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
using ContaCorrente.Domain.Entities;
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
    public class ObterTarifasCobradasHandler : IRequestHandler<ObterTarifasCobradasQuery, IEnumerable<TarifaCobradaResponse>>
    {
        private readonly ITarifaCobradaRepository _tarifaCobradaRepository;

        public ObterTarifasCobradasHandler(ITarifaCobradaRepository tarifaCobradaRepository)
        {
            _tarifaCobradaRepository = tarifaCobradaRepository;
        }

        public async Task<IEnumerable<TarifaCobradaResponse>> Handle(ObterTarifasCobradasQuery request, CancellationToken cancellationToken)
        {
            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (!string.IsNullOrEmpty(request.DataInicio))
            {
                if (!DateTime.TryParseExact(request.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataInicioConvertida))
                {
                    throw new ArgumentException("Data de início deve estar no formato DD/MM/YYYY");
                }
                dataInicio = dataInicioConvertida;
            }

            if (!string.IsNullOrEmpty(request.DataFim))
            {
                if (!DateTime.TryParseExact(request.DataFim, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataFimConvertida))
                {
                    throw new ArgumentException("Data de fim deve estar no formato DD/MM/YYYY");
                }
                dataFim = dataFimConvertida;
            }

            var tarifasCobradas = await _tarifaCobradaRepository.ObterPorContaAsync(request.IdConta, dataInicio, dataFim);

            return tarifasCobradas.Select(tarifa => new TarifaCobradaResponse(
                tarifa.IdTarifaCobrada,
                tarifa.IdContaCorrente,
                tarifa.IdTarifa,
                tarifa.TipoOperacao,
                tarifa.ValorTarifa,
                tarifa.Descricao,
                tarifa.DataCobranca,
                tarifa.IdOperacaoRelacionada
            ));
        }
    }
}
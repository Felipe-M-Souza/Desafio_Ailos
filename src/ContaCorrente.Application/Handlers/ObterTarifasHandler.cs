using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Handlers
{
    public class ObterTarifasHandler : IRequestHandler<ObterTarifasQuery, IEnumerable<TarifaResponse>>
    {
        private readonly ITarifaRepository _tarifaRepository;

        public ObterTarifasHandler(ITarifaRepository tarifaRepository)
        {
            _tarifaRepository = tarifaRepository;
        }

        public async Task<IEnumerable<TarifaResponse>> Handle(ObterTarifasQuery request, CancellationToken cancellationToken)
        {
            var tarifas = await _tarifaRepository.ObterTodasAsync();

            return tarifas.Select(tarifa => new TarifaResponse(
                tarifa.IdTarifa,
                tarifa.TipoOperacao,
                tarifa.Valor,
                tarifa.Descricao,
                tarifa.Ativa,
                tarifa.DataCriacao
            ));
        }
    }
}
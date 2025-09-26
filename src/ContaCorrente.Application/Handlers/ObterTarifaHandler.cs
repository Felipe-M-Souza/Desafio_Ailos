using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Queries;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Handlers
{
    public class ObterTarifaHandler : IRequestHandler<ObterTarifaQuery, TarifaResponse?>
    {
        private readonly ITarifaRepository _tarifaRepository;

        public ObterTarifaHandler(ITarifaRepository tarifaRepository)
        {
            _tarifaRepository = tarifaRepository;
        }

        public async Task<TarifaResponse?> Handle(ObterTarifaQuery request, CancellationToken cancellationToken)
        {
            var tarifa = await _tarifaRepository.ObterPorIdAsync(request.Id);
            
            if (tarifa == null)
            {
                return null;
            }

            return new TarifaResponse(
                tarifa.IdTarifa,
                tarifa.TipoOperacao,
                tarifa.Valor,
                tarifa.Descricao,
                tarifa.Ativa,
                tarifa.DataCriacao
            );
        }
    }
}


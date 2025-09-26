using ContaCorrente.Application.Commands;
using ContaCorrente.Application.DTOs;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Handlers
{
    public class CriarTarifaHandler : IRequestHandler<CriarTarifaCommand, TarifaResponse>
    {
        private readonly ITarifaRepository _tarifaRepository;

        public CriarTarifaHandler(ITarifaRepository tarifaRepository)
        {
            _tarifaRepository = tarifaRepository;
        }

        public async Task<TarifaResponse> Handle(CriarTarifaCommand request, CancellationToken cancellationToken)
        {
            // Verificar se já existe tarifa para este tipo de operação
            if (await _tarifaRepository.ExisteTarifaParaTipoAsync(request.TipoOperacao))
            {
                throw new InvalidOperationException($"Já existe uma tarifa para o tipo de operação: {request.TipoOperacao}");
            }

            // Validar dados
            if (request.Valor < 0)
            {
                throw new ArgumentException("Valor da tarifa não pode ser negativo");
            }

            if (string.IsNullOrWhiteSpace(request.Descricao))
            {
                throw new ArgumentException("Descrição é obrigatória");
            }

            // Criar nova tarifa
            var tarifa = new Tarifa(request.TipoOperacao, request.Valor, request.Descricao);
            var tarifaCriada = await _tarifaRepository.CriarAsync(tarifa);

            return new TarifaResponse(
                tarifaCriada.IdTarifa,
                tarifaCriada.TipoOperacao,
                tarifaCriada.Valor,
                tarifaCriada.Descricao,
                tarifaCriada.Ativa,
                tarifaCriada.DataCriacao
            );
        }
    }
}



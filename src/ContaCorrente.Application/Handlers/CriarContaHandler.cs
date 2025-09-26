using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Constants;
using ContaCorrente.Application.DTOs;
using ContaCorrente.Application.Utils;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ContaCorrente.Application.Handlers
{
    public class CriarContaHandler : IRequestHandler<CriarContaCommand, ContaResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;

        public CriarContaHandler(IContaCorrenteRepository contaRepository)
        {
            _contaRepository = contaRepository;
        }

        public async Task<ContaResponse> Handle(CriarContaCommand request, CancellationToken cancellationToken)
        {
            // Validar CPF
            if (!CpfValidator.IsValid(request.Cpf))
            {
                throw new ArgumentException(ErrorMessages.INVALID_DOCUMENT, "Cpf");
            }

            // Verificar se já existe uma conta com o mesmo número
            if (await _contaRepository.ExisteNumeroAsync(request.Numero))
            {
                throw new InvalidOperationException(ErrorMessages.CONTA_EXISTENTE);
            }

            // Verificar se já existe uma conta com o mesmo CPF
            if (await _contaRepository.ExisteCpfAsync(request.Cpf))
            {
                throw new InvalidOperationException("Já existe uma conta com este CPF");
            }

            // Gerar salt e hash da senha
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha, salt);

            // Criar nova conta
            var conta = new Conta(request.Numero, request.Nome, request.Cpf, senhaHash, salt);
            var contaCriada = await _contaRepository.CriarAsync(conta);

            return new ContaResponse(
                contaCriada.IdContaCorrente,
                contaCriada.Numero,
                contaCriada.Nome,
                contaCriada.Ativo
            );
        }
    }
}

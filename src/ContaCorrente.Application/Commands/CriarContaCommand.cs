using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Commands
{
    public record CriarContaCommand(int Numero, string Nome, string Cpf, string Senha) : IRequest<ContaResponse>;
}


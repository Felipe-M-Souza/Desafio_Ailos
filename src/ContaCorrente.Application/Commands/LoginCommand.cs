using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Commands
{
    public record LoginCommand(int? Numero, string? Cpf, string Senha) : IRequest<LoginResponse>;
}


using MediatR;

namespace ContaCorrente.Application.Commands
{
    public record InativarContaCommand(string IdConta, string Senha) : IRequest;
}

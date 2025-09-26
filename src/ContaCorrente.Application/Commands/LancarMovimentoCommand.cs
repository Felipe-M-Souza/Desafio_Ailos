using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Commands
{
    public record LancarMovimentoCommand(string IdConta, string Data, char Tipo, decimal Valor) : IRequest<LancarMovimentoResponse>;
}



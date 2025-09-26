using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Commands
{
    public record CriarTarifaCommand(string TipoOperacao, decimal Valor, string Descricao) : IRequest<TarifaResponse>;
}
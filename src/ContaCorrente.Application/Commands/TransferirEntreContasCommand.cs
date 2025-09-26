using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Commands
{
    public class TransferirEntreContasCommand : IRequest<TransferirEntreContasResponse>
    {
        public string IdContaOrigem { get; }
        public int NumeroContaDestino { get; }
        public decimal Valor { get; }
        public string Data { get; }
        public string? Descricao { get; }

        public TransferirEntreContasCommand(
            string idContaOrigem, 
            int numeroContaDestino, 
            decimal valor, 
            string data, 
            string? descricao = null)
        {
            IdContaOrigem = idContaOrigem;
            NumeroContaDestino = numeroContaDestino;
            Valor = valor;
            Data = data;
            Descricao = descricao;
        }
    }
}

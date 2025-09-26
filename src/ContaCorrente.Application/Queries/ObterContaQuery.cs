using ContaCorrente.Application.DTOs;
using MediatR;

namespace ContaCorrente.Application.Queries
{
    public class ObterContaQuery : IRequest<ContaResponse?>
    {
        public string Id { get; }

        public ObterContaQuery(string id)
        {
            Id = id;
        }
    }
}

using MediatR;

namespace Catalog.Application.Commands
{
    public record DeleteProductByIdCommand(string ProductId) : IRequest<bool>;
   }

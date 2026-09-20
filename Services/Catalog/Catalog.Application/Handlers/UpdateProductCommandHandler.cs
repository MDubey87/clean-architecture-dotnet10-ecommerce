using Catalog.Application.Commands;
using Catalog.Application.Mappers;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(request.Id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with Id {request.Id} not found.");
            // Fetch Brand and Type from the repository
            var brand = await _productRepository.GetBrandByIdAsync(request.BrandId);
            if (brand == null)
                throw new ArgumentException($"Invalid Brand Specified.");

            var type = await _productRepository.GetTypeByIdAsync(request.TypeId);
            if (type == null)
                throw new ArgumentException($"Invalid Type Specified.");

            // Create a new product entity
            // Update the existing product with the new values
            var updatedProduct = request.ToUpdateEntity(existingProduct, brand, type);
            return await _productRepository.UpdateProductAsync(updatedProduct);
        }
    }
}

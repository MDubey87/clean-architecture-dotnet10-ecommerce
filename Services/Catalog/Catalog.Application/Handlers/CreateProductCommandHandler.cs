using Catalog.Application.Commands;
using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Fetch Brand and Type from the repository
            var brand = await _productRepository.GetBrandByIdAsync(request.BrandId);
            if (brand == null)
                throw new ArgumentException($"Invalid Brand Specified.");
            
            var type = await _productRepository.GetTypeByIdAsync(request.TypeId);
            if (type == null)
                throw new ArgumentException($"Invalid Type Specified.");
            
            // Create a new product entity
            var productEntity = request.ToEntity(brand, type);
            // Save the product entity to the repository
            var newProduct = await _productRepository.CreateProductAsync(productEntity);
            // Return the response
            return newProduct.ToResponse();
        }
    }
}

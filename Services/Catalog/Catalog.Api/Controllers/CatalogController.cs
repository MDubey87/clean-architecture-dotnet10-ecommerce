using Catalog.Application.Commands;
using Catalog.Application.DTOs;
using Catalog.Application.Mappers;
using Catalog.Application.Queries;
using Catalog.Core.Specifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [Route("api/catalog")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("products")]
        public async Task<ActionResult<Pagination<ProductDto>>> GetAllProducts([FromQuery] CatalogSpecParams catalogSpecParams)
        {
            var query = new GetAllProductsQuery(catalogSpecParams);
            var result = await _mediator.Send(query);
            return Ok(result.ToDtoPagination());
        }

        [HttpGet("products/{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById([FromRoute] string id)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result.ToDto());
        }

        [HttpGet("products/by-name/{name}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByName([FromRoute] string name)
        {
            var query = new GetProductsByNameQuery(name);
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }
            var dtoList = result.Select(p => p.ToDto()).ToList();
            return Ok(dtoList);
        }

        [HttpPost("products")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            var command = createProductDto.ToCommand();
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateProduct), result);
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] string id)
        {
            var command = new DeleteProductByIdCommand(id);
            var result = await _mediator.Send(command);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] string id, [FromBody] UpdateProductDto updateProductDto)
        {
            var updateCommand = updateProductDto.ToCommand(id);
            var result = await _mediator.Send(updateCommand);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
        {
            var query = new GetAllBrandsQuery();
            var result = await _mediator.Send(query);
            var brandDtoList = result.Select(b => b.ToDto()).ToList();
            return Ok(brandDtoList);
        }
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<TypeDto>>> GetTypes()
        {
            var query = new GetAllTypesQuery();
            var result = await _mediator.Send(query);
            var typeDtoList = result.Select(t => t.ToDto()).ToList();
            return Ok(typeDtoList);
        }

        [HttpGet("products/brand/{brandName}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByBrandName([FromRoute]string brand)
        {
            //First get the products
            var query = new GetProductsByBrandQuery(brand);
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }
            var dtoList = result.Select(p => p.ToDto()).ToList();
            return Ok(dtoList);
        }
    }
}

using Basket.Application.Commands;
using Basket.Application.DTOs;
using Basket.Application.Mappers;
using Basket.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Api.Controllers
{
    [Route("api/v1/basket")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BasketController(IMediator mediator)
        {
            _mediator = mediator; 
        }
        [HttpGet("{userName}")]
        public async Task<ActionResult<ShoppingCartDto>> GetBasket(string userName)
        {
            var query = new GetBasketByUserNameQuery(userName);
            var result = await _mediator.Send(query);
            return Ok(result.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCartDto>> CreateOrUpdateBasket([FromBody] CreateShoppingCartCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result.ToDto());
        }

        [HttpDelete("{userName}")]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            var cmd = new DeleteBasketByUserNameCommand(userName);
            await _mediator.Send(cmd);
            return Ok();
        }
    }
}

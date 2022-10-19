using Basket.api.Entities;
using Basket.api.GrpcServices;
using Basket.api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly DiscountGrpcService _discountGrpcService;

        public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService)
        {
            _basketRepository = basketRepository;
            _discountGrpcService = discountGrpcService;
        }

        [HttpGet("{username}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
        {
            var basket = await _basketRepository.GetUserBasket(username);
            return Ok(basket ?? new ShoppingCart(username));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            //todo: get date from dicsount.grpc and calculate final price of product
            foreach (var item in basket.Items)
            {
               var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
            
            return Ok(await _basketRepository.UpdateBasket(basket));
        }

        [HttpDelete("{username}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string username)
        {
            await _basketRepository.DeleteBasket(username);
            return Ok();
        }
    }
}

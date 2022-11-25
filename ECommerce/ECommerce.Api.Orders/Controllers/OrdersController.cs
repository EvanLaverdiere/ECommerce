using ECommerce.Api.Orders.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Orders.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController: ControllerBase
    {
        private readonly IOrdersProvider ordersProvider;
        public OrdersController(IOrdersProvider ordersProvider)
        {
            this.ordersProvider = ordersProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync()
        {
            var result = await ordersProvider.GetOrdersAsync();
            if (result.IsSuccess)
                return Ok(result.Orders);
            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdersAsync(int id)
        {
            var result = await ordersProvider.GetOrdersAsync(id);
            if (result.IsSuccess)
                return Ok(result.Orders);
            return NotFound();
        }
    }
}

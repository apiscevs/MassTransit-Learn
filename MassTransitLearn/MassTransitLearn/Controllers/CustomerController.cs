using MassTransit;
using MassTransitLearn.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MassTransitLearn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CustomerController(ILogger<OrderController> logger,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        [HttpDelete]
        public async Task<IActionResult> Post(Guid id, string customerNumber)
        {
            await _publishEndpoint.Publish<CustomerAccountClosed>(new
            {
                CustomerId = id,
                CustomerNumber = customerNumber
            });

            return Accepted();            
        }
    }
}

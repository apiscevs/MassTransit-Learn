using MassTransit;
using MassTransitLearn.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitLearn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IRequestClient<SubmitOrder> _submitOrderRequestClient;
        private readonly ISendEndpointProvider _submitOrderEndpointProvider;

        public OrderController(ILogger<OrderController> logger,
            IRequestClient<SubmitOrder> submitOrderRequestClient,
            ISendEndpointProvider submitOrderEndpointProvider)
        {
            _logger = logger;
            _submitOrderRequestClient = submitOrderRequestClient;
            _submitOrderEndpointProvider = submitOrderEndpointProvider;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Guid id, string customerNumber)
        {
            var (accepted, rejected) = await _submitOrderRequestClient
                .GetResponse<OrderSubmitionAccepted, OrderSubmitionRejected>(new
            {
                OrderId = id,
                Timestamp = InVar.Timestamp,
                CustomerNumber = customerNumber
            });

            if (accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;
                return Accepted(response.Message);
            }
            else
            {
                var response = await rejected;
                return BadRequest(response.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid id, string customerNumber)
        {
            var endpoint = await _submitOrderEndpointProvider.GetSendEndpoint(new Uri("exchange:submit-order"));

            await endpoint.Send<SubmitOrder>(new
            {
                OrderId = id,
                Timestamp = InVar.Timestamp,
                CustomerNumber = customerNumber
            });
            return Accepted();
        }
    }
}

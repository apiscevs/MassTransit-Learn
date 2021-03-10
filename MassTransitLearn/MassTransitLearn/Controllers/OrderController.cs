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
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IRequestClient<SubmitOrder> _submitOrderRequestClient;
        private readonly IRequestClient<CheckOrder> _checkOrderClient;
        private readonly ISendEndpointProvider _submitOrderEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderController(ILogger<OrderController> logger,
            IRequestClient<SubmitOrder> submitOrderRequestClient,
            IRequestClient<CheckOrder> checkOrderClient,
            ISendEndpointProvider submitOrderEndpointProvider,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _submitOrderRequestClient = submitOrderRequestClient;
            _checkOrderClient = checkOrderClient;
            _submitOrderEndpointProvider = submitOrderEndpointProvider;
            _publishEndpoint = publishEndpoint;
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

        [HttpPatch]
        public async Task<IActionResult> Patch(Guid id)
        {
            await _publishEndpoint.Publish<OrderAccepted>(new
            {
                OrderId = id,
                Timestamp = InVar.Timestamp
            });

            return Accepted();
        }

        [HttpGet]
        public async Task<ActionResult> Get(Guid id)
        {
            var (status, notFound) = await _checkOrderClient.GetResponse<OrderStatus, OrderNotFound>(new
            {
                OrderId = id
            });

            if (status.IsCompletedSuccessfully)
            {
                var response = await status;
                return Ok(response.Message);
            } else
            {
                var response = await notFound;
                return NotFound(response.Message);
            }
        }
    }
}

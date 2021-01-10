using MassTransit;
using MassTransitLearn.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassTransitLearn.Components.Consumers
{
    public class SubmitOrderConsumer :
        IConsumer<SubmitOrder>
    {
        private ILogger<SubmitOrderConsumer> _logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            _logger.Log(LogLevel.Debug, "SubmitOrderConsumer: {CustomerNumber}", context.Message.CustomerNumber);

            if (context.ResponseAddress != null)
            {
                if (context.Message.CustomerNumber.ToLowerInvariant().Contains("test"))
                {
                    await context.RespondAsync<OrderSubmitionRejected>(new
                    {
                        InVar.Timestamp,
                        OrderId = context.Message.OrderId,
                        CustomerNumber = context.Message.CustomerNumber,
                        Reason = $"Test customers cannot receive orders: {context.Message.CustomerNumber}"
                    });
                    return;
                }
                await context.RespondAsync<OrderSubmitionAccepted>(new
                {
                    InVar.Timestamp,
                    OrderId = context.Message.OrderId,
                    CustomerNumber = context.Message.CustomerNumber
                });
            } else
            {
                //just save, no response
            }
        }
    }
}

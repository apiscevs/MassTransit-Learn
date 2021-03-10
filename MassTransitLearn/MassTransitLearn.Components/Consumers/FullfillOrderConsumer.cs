using MassTransit;
using MassTransit.Courier;
using MassTransitLearn.Contracts;
using System;
using System.Threading.Tasks;

namespace MassTransitLearn.Components.Consumers
{
    public class FullfillOrderConsumer : IConsumer<FullfillOrder>
    {
        public async Task Consume(ConsumeContext<FullfillOrder> context)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity("AllocateInventory", new Uri("queue:allocate-inventory_execute"), new
            {
                ItemNumber = "ItemNumber 123",
                Quantity = 10
            });

            builder.AddVariable("OrderId", context.Message.OrderId);

            var routingSlip = builder.Build();

            await context.Execute(routingSlip);
        }
    }
}

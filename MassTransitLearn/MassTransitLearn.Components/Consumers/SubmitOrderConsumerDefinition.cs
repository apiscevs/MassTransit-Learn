using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using GreenPipes;

namespace MassTransitLearn.Components.Consumers
{
    public class SubmitOrderConsumerDefinition : ConsumerDefinition<SubmitOrderConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(3, 1000));
        }
    }
}

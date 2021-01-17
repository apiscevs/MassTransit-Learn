using MassTransit;
using MassTransit.Definition;
using GreenPipes;

namespace MassTransitLearn.Components.StateMachine
{
    public class OrderStateMachineDefinition : SagaDefinition<OrderState>
    {
        public OrderStateMachineDefinition()
        {
            ConcurrentMessageLimit = 4;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(t => t.Interval(5, 5000));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}

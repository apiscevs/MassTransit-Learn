
using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;
using MassTransit;
using MassTransitLearn.Contracts;

namespace MassTransitLearn.Components.StateMachine.OrderStateMachineActivities
{
    public class AcceptOrderActivity : Activity<OrderState, OrderAccepted>
    {
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, OrderAccepted> context, Behavior<OrderState, OrderAccepted> next)
        {
            Console.WriteLine($"Hello from activity {context.Data.OrderId}");

            var consumeContext = context.GetPayload<ConsumeContext>();

            var sendEndpoint = await consumeContext.GetSendEndpoint(new Uri("exchange:fullfill-order"));

            await sendEndpoint.Send<FullfillOrder>(new
            {
                OrderId = context.Data.OrderId
            });

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAccepted, TException> context, Behavior<OrderState, OrderAccepted> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("accept-order");
        }
    }
}

using Automatonymous;
using MassTransit;
using MassTransitLearn.Contracts;
using System;

namespace MassTransitLearn.Components.StateMachine
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            //setup what property to use to correlate message
            Event(() => OrderSubmittedEvent, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderStatusRequestedEvent, x => {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<OrderNotFound>(new { context.Message.OrderId });
                    }
                }));
            });

            //setup to store current state in this property
            InstanceState(x => x.CurrentState);

            Initially(
                When(OrderSubmittedEvent)
                .Then(context => {
                    context.Instance.CustomerNumber = context.Data.CustomerNumber;
                    context.Instance.UpdateDatetime = DateTime.UtcNow;
                    context.Instance.SubmitDatetime = context.Data.Timestamp;
                })
                .TransitionTo(OrderSubmitedState));

            During(OrderSubmitedState, Ignore(OrderSubmittedEvent));

            DuringAny(
                When(OrderSubmittedEvent)
                .Then(context =>
                {
                    context.Instance.CustomerNumber ??= context.Data.CustomerNumber;
                    context.Instance.SubmitDatetime ??= context.Data.Timestamp;
                }));

            DuringAny(
                When(OrderStatusRequestedEvent)
                .RespondAsync(x => x.Init<OrderStatus>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    State = x.Instance.CurrentState,
                    CustomerNumber = x.Instance.CustomerNumber
                })));
        }

        public State OrderSubmitedState { get; private set; }
        public Event<OrderSubmitted> OrderSubmittedEvent { get; private set; }
        public Event<CheckOrder> OrderStatusRequestedEvent { get; private set; }
    }
}

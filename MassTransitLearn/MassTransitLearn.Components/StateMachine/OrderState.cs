using Automatonymous;
using MassTransit.RedisIntegration;
using System;

namespace MassTransitLearn.Components.StateMachine
{
    public class OrderState : SagaStateMachineInstance, IVersionedSaga
    {
        public int Version { get; set; }
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public DateTime? SubmitDatetime { get; set; }
    }
}
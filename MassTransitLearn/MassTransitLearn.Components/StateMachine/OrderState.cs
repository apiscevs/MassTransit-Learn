using Automatonymous;
using MassTransit.MongoDbIntegration.Saga;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MassTransitLearn.Components.StateMachine
{
    public class OrderState : SagaStateMachineInstance, IVersionedSaga
    {
        public int Version { get; set; }
        [BsonId]
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime? UpdateDatetime { get; set; }
        public DateTime? SubmitDatetime { get; set; }
    }
}
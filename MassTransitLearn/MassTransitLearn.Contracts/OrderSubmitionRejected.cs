using System;

namespace MassTransitLearn.Contracts
{
    public interface OrderSubmitionRejected
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
        string Reason { get; }
    }

}

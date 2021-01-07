using System;

namespace MassTransitLearn.Contracts
{
    public interface OrderSubmitionAccepted 
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }
    }

}

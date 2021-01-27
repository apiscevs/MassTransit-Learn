using System;

namespace MassTransitLearn.Contracts
{
    public interface OrderAccepted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
    }
}

using System;

namespace MassTransitLearn.Contracts
{
    public interface OrderStatus
    {
        Guid OrderId { get; set; }
        string State { get; }
        string CustomerNumber { get; }
    }
}

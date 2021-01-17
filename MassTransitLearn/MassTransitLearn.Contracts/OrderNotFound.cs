using System;

namespace MassTransitLearn.Contracts
{
    public interface OrderNotFound
    {
        Guid OrderId { get; }
    }
}

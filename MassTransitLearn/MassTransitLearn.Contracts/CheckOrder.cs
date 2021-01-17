using System;

namespace MassTransitLearn.Contracts
{
    public interface CheckOrder
    {
        Guid OrderId { get; }
    }
}

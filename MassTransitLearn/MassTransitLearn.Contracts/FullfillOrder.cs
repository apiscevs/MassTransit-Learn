using System;

namespace MassTransitLearn.Contracts
{
    public interface FullfillOrder
    {
        Guid OrderId { get; }
    }
}

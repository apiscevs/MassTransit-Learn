using System;

namespace MassTransitLearn.Contracts
{
    public interface CustomerAccountClosed
    {
        Guid CustomerId { get; }
        string CustomerNumber { get; }
    }
}

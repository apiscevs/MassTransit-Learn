using System;

namespace Warehouse.Contracts
{
    public interface ReleaseAllocationRequested
    {
        Guid AllocationId { get; }
        string Reason { get; }
    }
}

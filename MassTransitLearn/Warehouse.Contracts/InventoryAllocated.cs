using System;

namespace Warehouse.Contracts
{
    public interface InventoryAllocated
    {
        Guid AllocayionId { get; }
        string ItemNumber { get; }
        decimal Quantity { get; }
    }
}

using MassTransit;
using MassTransit.Courier;
using System;
using System.Threading.Tasks;
using Warehouse.Contracts;

namespace MassTransitLearn.Components.CourierActivities
{
    public class AllocateInventoryActivity : IActivity<AllocateInventoryArguments, AllocateInventoryLog>
    {
        readonly IRequestClient<AllocateInventory> _client;

        public AllocateInventoryActivity(IRequestClient<AllocateInventory> client)
        {
            _client = client;
        }

        public async Task<CompensationResult> Compensate(CompensateContext<AllocateInventoryLog> context)
        {
            await context.Publish<ReleaseAllocationRequested>(new
            {
                AllocationId = context.Log.AllocationId,
                Reason = "Order faulted"
            });

            return context.Compensated();
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<AllocateInventoryArguments> context)
        {
            var orderId = context.Arguments.OrderId;
            var quantity = context.Arguments.Quantity;
            var itemNumber = context.Arguments.ItemNumber;

            if (string.IsNullOrEmpty(itemNumber))
                throw new ArgumentNullException(nameof(itemNumber));

            if (quantity <= 0)
                throw new ArgumentNullException(nameof(quantity));

            var allocationId = NewId.NextGuid();

            var response = await _client.GetResponse<InventoryAllocated>(new
            {
                AllocationId = allocationId,
                ItemNumber = quantity,
                Quantity = quantity
            });

            return context.Completed(new
            {
                Allocation = allocationId
            });
        }
    }

    public interface AllocateInventoryArguments
    {
        Guid OrderId { get; }
        string ItemNumber { get; }
        decimal Quantity { get; }
    }

    public interface AllocateInventoryLog
    {
        Guid AllocationId { get; }
    }
}

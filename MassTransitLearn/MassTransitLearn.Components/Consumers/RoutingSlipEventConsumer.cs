using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitLearn.Components.Consumers
{
    public class RoutingSlipEventConsumer : 
        IConsumer<RoutingSlipCompleted>,
        IConsumer<RoutingSlipActivityCompleted>,
        IConsumer<RoutingSlipFaulted>
    {
        private readonly ILogger<RoutingSlipEventConsumer> _logger;

        public RoutingSlipEventConsumer(ILogger<RoutingSlipEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Log(LogLevel.Information, "Routing slip completed: {TrackingNumber}", context.Message.TrackingNumber);

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Log(LogLevel.Information, "Routing slip activity completed: {TrackingNumber} {ActivityName}", context.Message.TrackingNumber, context.Message.ActivityName);

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.Log(LogLevel.Information, "Routing slip activity completed: {TrackingNumber} {ExceptionInfo}", 
                    context.Message.TrackingNumber, 
                    context.Message.ActivityExceptions.FirstOrDefault());

            return Task.CompletedTask;
        }
    }
}

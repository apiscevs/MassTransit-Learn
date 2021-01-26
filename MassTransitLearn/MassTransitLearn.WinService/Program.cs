using MassTransit;
using MassTransit.Definition;
using MassTransitLearn.Components.Consumers;
using MassTransitLearn.Components.StateMachine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MassTransitLearn.WinService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || string.Concat(args).Contains("--console"));

            var builder = new HostBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", true);
                config.AddEnvironmentVariables();

                if (args != null)
                    config.AddCommandLine(args);
            })
            .ConfigureServices((hostingContext, services) =>
            {
                services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

                services.AddMassTransit(cfg =>
                {
                    cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

                    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
                    .MongoDbRepository(t=> {
                        t.Connection = "mongodb://127.0.0.1";
                        t.DatabaseName = "orderdb";
                    });

                    cfg.AddBus(ConfigureBus);
                });

                 services.AddHostedService<MassTransitConsoleHostedService>();
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
            });

            if (isService)
                await builder.UseWindowsService().Build().RunAsync();
            else
                await builder.RunConsoleAsync();
        }

        static IBusControl ConfigureBus(IBusRegistrationContext provider)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ConfigureEndpoints(provider);
            });
        }
    }
}

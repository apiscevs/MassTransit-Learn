using MassTransit;
using MassTransitLearn.Components.Consumers;
using MassTransitLearn.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransitLearn
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var inMemoryBus = Bus.Factory.CreateUsingInMemory(busFactoryConfig =>{
            //    busFactoryConfig.ReceiveEndpoint("queue_name", ep =>
            //    {
            //        //configure the endpoint
            //    });
            //});

            //services.AddMassTransit(config =>
            //{
            //    config.AddBus(provider => inMemoryBus);
            //    config.AddConsumer<SubmitOrderConsumer>();
            //    config.AddRequestClient<SubmitOrder>();
            //});


            //services.AddMassTransit(cfg =>
            //{
            //    cfg.AddConsumer<SubmitOrderConsumer>();
            //    cfg.AddRequestClient<SubmitOrder>();
            //    cfg.UsingInMemory();
            //});
            services.AddMediator(x =>
            {
                x.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
                x.AddRequestClient<SubmitOrder>();
            });


            // Register the Swagger services
            services.AddSwaggerDocument();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

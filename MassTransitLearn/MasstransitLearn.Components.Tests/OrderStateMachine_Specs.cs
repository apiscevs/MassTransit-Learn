using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using MassTransit;
using MassTransit.Testing;
using MassTransitLearn.Components.StateMachine;
using MassTransitLearn.Contracts;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MasstransitLearn.Components.Tests
{
    [TestFixture]
    public class OrderStateMachine_Specs
    {
        [Test]
        public async Task Should_create_a_state_instanceAsync()
        {
            var orderStateMachine = new OrderStateMachine();
            var harness = new InMemoryTestHarness();
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(new OrderStateMachine());
            await harness.Start();

            try
            {
                var orderId = NewId.NextGuid();
                await harness.Bus.Publish<OrderSubmitted>(new
                {
                    OrderId = orderId,
                    Timestamp = InVar.Timestamp,
                    CustomerNumber = "cust number"
                });

                var instanceId = await saga.Exists(orderId, t => t.OrderSubmitedState);
                Assert.That(instanceId, Is.Not.Null);

                var instance = saga.Sagas.Contains(instanceId.Value);
                Assert.That(instance.CustomerNumber, Is.EqualTo("cust number"));
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_respond_to_getStatus()
        {
            var orderStateMachine = new OrderStateMachine();
            var harness = new InMemoryTestHarness();
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(new OrderStateMachine());
            await harness.Start();

            try
            {
                var orderId = NewId.NextGuid();
                await harness.Bus.Publish<OrderSubmitted>(new
                {
                    OrderId = orderId,
                    Timestamp = InVar.Timestamp,
                    CustomerNumber = "cust number"
                });

                var instanceId = await saga.Exists(orderId, t => t.OrderSubmitedState);
                Assert.That(instanceId, Is.Not.Null);

                var instance = saga.Sagas.Contains(instanceId.Value);
                Assert.That(instance.CustomerNumber, Is.EqualTo("cust number"));

                var requestClient = await harness.ConnectRequestClient<CheckOrder>();
                var response = await requestClient.GetResponse<OrderStatus>(new
                {
                    OrderId = orderId
                });

                Assert.That(response.Message.State, Is.EqualTo(orderStateMachine.OrderSubmitedState.Name));
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_cancel_when_customer_account_closed()
        {
            var orderStateMachine = new OrderStateMachine();
            var harness = new InMemoryTestHarness();
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(new OrderStateMachine());
            await harness.Start();

            try
            {
                var orderId = NewId.NextGuid();
                await harness.Bus.Publish<OrderSubmitted>(new
                {
                    OrderId = orderId,
                    Timestamp = InVar.Timestamp,
                    CustomerNumber = "cust number"
                });

                var instanceId = await saga.Exists(orderId, t => t.OrderSubmitedState);
                Assert.That(instanceId, Is.Not.Null);

                var instance = saga.Sagas.Contains(instanceId.Value);
                Assert.That(instance.CustomerNumber, Is.EqualTo("cust number"));

                await harness.Bus.Publish<CustomerAccountClosed>(new
                {
                    CustomerId = InVar.Id,
                    CustomerNumber = "cust number"
                });

                instanceId = await saga.Exists(orderId, t => t.CanceledState);

                Assert.That(instanceId, Is.Not.Null);

            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_accept_when_order_is_accepted()
        {
            var orderStateMachine = new OrderStateMachine();
            var harness = new InMemoryTestHarness();
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(new OrderStateMachine());
            await harness.Start();

            try
            {
                var orderId = NewId.NextGuid();
                await harness.Bus.Publish<OrderSubmitted>(new
                {
                    OrderId = orderId,
                    Timestamp = InVar.Timestamp,
                    CustomerNumber = "cust number"
                });

                var instanceId = await saga.Exists(orderId, t => t.OrderSubmitedState);
                Assert.That(instanceId, Is.Not.Null);

                var instance = saga.Sagas.Contains(instanceId.Value);
                Assert.That(instance.CustomerNumber, Is.EqualTo("cust number"));

                await harness.Bus.Publish<OrderAccepted>(new
                {
                    OrderId = orderId,
                    Timestamp = InVar.Timestamp
                });

                instanceId = await saga.Exists(orderId, t => t.AcceptedState);

                Assert.That(instanceId, Is.Not.Null);

            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public void Show_me_the_state_machine_graph()
        {
            var orderStateMachine = new OrderStateMachine();
            var graph = orderStateMachine.GetGraph();
            var generator = new StateMachineGraphvizGenerator(graph);
            string dots = generator.CreateDotFile();
            Console.WriteLine(dots);
        }
    }
}

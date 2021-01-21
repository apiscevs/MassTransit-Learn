using MassTransit;
using MassTransit.Testing;
using MassTransitLearn.Components.StateMachine;
using MassTransitLearn.Contracts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}

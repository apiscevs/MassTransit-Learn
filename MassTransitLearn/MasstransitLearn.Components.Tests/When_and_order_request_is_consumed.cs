using MassTransit;
using MassTransit.Testing;
using MassTransitLearn.Components.Consumers;
using MassTransitLearn.Contracts;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitLearn.Components.Tests
{
    [TestFixture]
    public class When_and_order_request_is_consumed
    {
        [Test]
        public async Task Should_consume_submit_order_command()
        {
            var harness = new InMemoryTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(5);
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();

            try
            {
                var orderId = NewId.NextGuid();

                var requestClient = await harness.ConnectRequestClient<SubmitOrder>();

                await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
                {
                    OrderId = orderId,
                    Timestamp = InVar.Timestamp,
                    CustomerNumber = "customer number name"
                });

                Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);
                Assert.That(harness.Sent.Select<OrderSubmitionAccepted>().Any(), Is.False);
                Assert.That(harness.Sent.Select<OrderSubmitionRejected>().Any(), Is.False);
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_respond_with_ok_if_ok()
        {
            var harness = new InMemoryTestHarness();
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();

            try
            {
                var orderId = NewId.NextGuid();

                var requestClient = await harness.ConnectRequestClient<SubmitOrder>();

                var response = await requestClient.GetResponse<OrderSubmitionAccepted>(
                    new
                    {
                        OrderId = orderId,
                        Timestamp = InVar.Timestamp,
                        CustomerNumber = "customer number name"
                    });

                Assert.That(response.Message.OrderId, Is.EqualTo(orderId));
                Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);
                Assert.That(harness.Sent.Select<OrderSubmitionAccepted>().Any(), Is.True);
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_respond_with_rejected_if_customer_name_contains_test()
        {
            var harness = new InMemoryTestHarness();
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();

            try
            {
                var orderId = NewId.NextGuid();

                var requestClient = await harness.ConnectRequestClient<SubmitOrder>();

                var response = await requestClient.GetResponse<OrderSubmitionRejected>(
                    new
                    {
                        OrderId = orderId,
                        Timestamp = InVar.Timestamp,
                        CustomerNumber = "customer number name test"
                    });

                Assert.That(response.Message.OrderId, Is.EqualTo(orderId));
                Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);
                Assert.That(harness.Sent.Select<OrderSubmitionRejected>().Any(), Is.True);
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_publish_order_submitted_event()
        {
            var harness = new InMemoryTestHarness();
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();

            try
            {
                var orderId = NewId.NextGuid();

                await harness.InputQueueSendEndpoint.Send<SubmitOrder>(
                    new
                    {
                        OrderId = orderId,
                        Timestamp = InVar.Timestamp,
                        CustomerNumber = "customer number name"
                    });

                Assert.That(harness.Published.Select<OrderSubmitted>().Any(), Is.True);
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}

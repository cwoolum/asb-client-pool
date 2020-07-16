using System.Threading.Tasks;

using Microsoft.Azure.ServiceBus;

using Moq;

using Turnoutt.Azure.ServiceBus.Core.Exceptions;
using Turnoutt.Azure.ServiceBus.Core.Messages;
using Turnoutt.Azure.ServiceBus.Core.UnitTests.TestMessages;

using Xunit;

namespace Turnoutt.Azure.ServiceBus.Core.UnitTests
{
    public class MappingTests
    {
        [Fact]
        public async Task EnsureExceptionIsThrowIfMessageIsNotMapped()
        {
            var builder = new ServiceBusClientPoolBuilder(new ServiceBusConnection("test.servicebus.windows.net", TransportType.Amqp, RetryPolicy.Default));

            var topicMock = new Mock<ITopicClient>();

            builder.AddTopicClient<FooMessage>(topicMock.Object);

            var pool = new ServiceBusClientPool(builder);

            await Assert.ThrowsAsync<MessageNotMappedException>(() =>
           {
               return pool.SendTopicMessageAsync(new BarMessage());
           });
        }

        [Fact]
        public void EnsureTopicClientCanBeFetched()
        {
            var builder = new ServiceBusClientPoolBuilder(new ServiceBusConnection(new ServiceBusConnectionStringBuilder("test.servicebus.windows.net", "test", "test", "test")));

            var topicMock = new Mock<ITopicClient>();
            topicMock.Setup(m => m.TopicName).Returns("Test");

            topicMock.Setup(m => m.SendAsync(It.IsAny<Message>()))
                .Verifiable();

            builder.AddTopicClient<FooMessage>(topicMock.Object);

            var pool = new ServiceBusClientPool(builder);

            var client = pool.GetTopicSubscriptionClient<FooMessage>("test");

            Assert.NotNull(client);
        }

        [Fact]
        public async Task EnsureTopicsCanBeMappedProperly()
        {
            var builder = new ServiceBusClientPoolBuilder(new ServiceBusConnection("test.servicebus.windows.net", TransportType.Amqp, RetryPolicy.Default));

            var topicMock = new Mock<ITopicClient>();

            topicMock.Setup(m => m.SendAsync(It.IsAny<Message>()))
                .Verifiable();

            builder.AddTopicClient<FooMessage>(topicMock.Object);

            var pool = new ServiceBusClientPool(builder);

            await pool.SendTopicMessageAsync(new FooMessage());
        }

        [Fact]
        public async Task EnsureTopicsWithJsonSerializedMessageCanBeMappedProperly()
        {
            var builder = new ServiceBusClientPoolBuilder(new ServiceBusConnection("test.servicebus.windows.net", TransportType.Amqp, RetryPolicy.Default));

            var topicMock = new Mock<ITopicClient>();
            topicMock.Setup(m => m.SendAsync(It.IsAny<Message>()))
                .Verifiable();

            builder.AddTopicClient<FooMessage>(topicMock.Object);

            var pool = new ServiceBusClientPool(builder);

            await pool.SendTopicMessageAsync(new JsonSerializedMessage<FooMessage>(new FooMessage()));
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.ServiceBus;

using Turnoutt.Azure.ServiceBus.Core.Messages;

namespace Turnoutt.Azure.ServiceBus.Core
{
    /// <summary>
    /// A client pool that allows you to send messages or get preregistered subscription clients.
    /// </summary>
    public interface IServiceBusClientPool
    {
        /// <summary>
        /// Gets the queue client so that message handlers can be registered
        /// </summary>
        /// <typeparam name="T">The message type that was registered</typeparam>
        /// <param name="subscriptionName"></param>
        /// <param name="receiveMode"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        IQueueClient GetQueueSubscriptionClient<T>();

        /// <summary>
        /// Gets the topic client so that message handlers can be registered
        /// </summary>
        /// <typeparam name="T">The message type that was registered</typeparam>
        /// <param name="subscriptionName"></param>
        /// <param name="receiveMode"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        SubscriptionClient GetTopicSubscriptionClient<T>(string subscriptionName, ReceiveMode receiveMode = ReceiveMode.PeekLock, RetryPolicy retryPolicy = null);

        Task SendQueueMessageAsync<T>(T message) where T : new();

        Task SendQueueMessageAsync<T>(JsonSerializedMessage<T> message) where T : new();

        Task SendQueueMessagesAsync<T>(IList<T> messageList) where T : new();

        Task SendQueueMessagesAsync<T>(IList<JsonSerializedMessage<T>> messageList) where T : new();

        Task SendTopicMessageAsync<T>(T message) where T : new();

        Task SendTopicMessageAsync<T>(JsonSerializedMessage<T> message) where T : new();

        Task SendTopicMessagesAsync<T>(IList<T> messageList) where T : new();

        Task SendTopicMessagesAsync<T>(IList<JsonSerializedMessage<T>> messageList) where T : new();
    }
}
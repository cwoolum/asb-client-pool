using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.ServiceBus;

using Turnoutt.Azure.ServiceBus.Core.Exceptions;
using Turnoutt.Azure.ServiceBus.Core.Messages;

namespace Turnoutt.Azure.ServiceBus.Core
{
    ///<inheritdoc cref="IServiceBusClientPool"/>
    internal class ServiceBusClientPool : IServiceBusClientPool
    {
        private readonly ServiceBusConnection _connection;
        private readonly IDictionary<Type, IQueueClient> _queueMappings;
        private readonly IDictionary<Type, ITopicClient> _topicMappings;

        public ServiceBusClientPool(ServiceBusClientPoolBuilder builder)
        {
            _topicMappings = builder.TopicMappings;
            _queueMappings = builder.QueueMappings;
            _connection = builder.Connection;
        }

       
        
        public SubscriptionClient GetTopicSubscriptionClient<T>(string subscriptionName, ReceiveMode receiveMode = ReceiveMode.PeekLock, RetryPolicy retryPolicy = null)
        {
            if (!_topicMappings.ContainsKey(typeof(T)))
            {
                throw new Exception("Topic has not been registered! Please ensure AddTopicClient has been called for the message type " + typeof(T).Name);
            }

            var topicMapping = _topicMappings[typeof(T)];

            return new SubscriptionClient(
                _connection,
                topicMapping.TopicName,
                subscriptionName,
                receiveMode,
                retryPolicy);
        }

       
        public SubscriptionClient GetQueueSubscriptionClient<T>(string subscriptionName, ReceiveMode receiveMode = ReceiveMode.PeekLock, RetryPolicy retryPolicy = null)
        {
            if (!_queueMappings.ContainsKey(typeof(T)))
            {
                throw new Exception("Queue has not been registered! Please ensure AddQueueClient has been called for the message type " + typeof(T).Name);
            }

            var topicMapping = _queueMappings[typeof(T)];

            return new SubscriptionClient(
                _connection,
                topicMapping.QueueName,
                subscriptionName,
                receiveMode,
                retryPolicy);
        }

        public Task SendQueueMessageAsync<T>(T message) where T : new()
        {
            return SendQueueMessageAsync(new T[1]
            {
                message
            });
        }

        public async Task SendQueueMessageAsync<T>(IList<T> messageList) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType()).Distinct();

            EnsureMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _queueMappings[message.GetType()];

                await mapping.SendAsync(new JsonSerializedMessage<T>(message));
            }
        }

        public Task SendQueueMessageAsync<T>(JsonSerializedMessage<T> message) where T : new()
        {
            return SendQueueMessageAsync(new JsonSerializedMessage<T>[1]
            {
                message
            });
        }

        public async Task SendQueueMessageAsync<T>(IList<JsonSerializedMessage<T>> messageList) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType()).Distinct();

            EnsureMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _queueMappings[message.GetType()];

                await mapping.SendAsync(message);
            }
        }

        public Task SendTopicMessageAsync<T>(T message) where T : new()
        {
            return SendTopicMessageAsync(new T[1]
            {
                message
            });
        }

        public async Task SendTopicMessageAsync<T>(IList<T> messageList) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType()).Distinct();

            EnsureMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _topicMappings[message.GetType()];

                await mapping.SendAsync(new JsonSerializedMessage<T>(message));
            }
        }

        public Task SendTopicMessageAsync<T>(JsonSerializedMessage<T> message) where T : new()
        {
            return SendTopicMessageAsync(new JsonSerializedMessage<T>[1]
            {
                message
            });
        }

        public async Task SendTopicMessageAsync<T>(IList<JsonSerializedMessage<T>> messageList) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType().GetGenericArguments()[0]).Distinct();
            EnsureMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _topicMappings[message.GetType().GetGenericArguments()[0]];

                await mapping.SendAsync(message);
            }
        }

        internal void EnsureMessagesAreMapped(IEnumerable<Type> messageTypes)
        {
            // Check that all of the message types have been registered first
            foreach (var messageType in messageTypes)
            {
                if (!_topicMappings.ContainsKey(messageType))
                {
                    throw new MessageNotMappedException($"There is no topic client registered for the message type {messageType}");
                }
            }
        }
    }
}
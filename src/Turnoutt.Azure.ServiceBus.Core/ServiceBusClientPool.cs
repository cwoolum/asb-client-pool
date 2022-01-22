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

        public IQueueClient GetQueueSubscriptionClient<T>()
        {
            if (!_queueMappings.ContainsKey(typeof(T)))
            {
                throw new Exception("Queue has not been registered! Please ensure AddQueueClient has been called for the message type " + typeof(T).Name);
            }

            var topicMapping = _queueMappings[typeof(T)];

            return topicMapping;
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

        public Task ScheduleQueueMessageAsync<T>(T message, DateTimeOffset scheduleEnqueueTimeUtc) where T : new()
        {
            return ScheduleQueueMessagesAsync(new T[1]
            {
                message
            }, scheduleEnqueueTimeUtc);
        }

        public Task ScheduleQueueMessageAsync<T>(JsonSerializedMessage<T> message, DateTimeOffset scheduleEnqueueTimeUtc) where T : new()
        {
            return ScheduleQueueMessagesAsync(new JsonSerializedMessage<T>[1]
           {
                message
           }, scheduleEnqueueTimeUtc);
        }

        public async Task ScheduleQueueMessagesAsync<T>(IList<T> messageList, DateTimeOffset scheduleEnqueueTimeUtc) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType()).Distinct();

            EnsureQueueMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _queueMappings[message.GetType()];

                await mapping.ScheduleMessageAsync(new JsonSerializedMessage<T>(message), scheduleEnqueueTimeUtc);
            }
        }

        public async Task ScheduleQueueMessagesAsync<T>(IList<JsonSerializedMessage<T>> messageList, DateTimeOffset scheduleEnqueueTimeUtc) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType()).Distinct();

            EnsureQueueMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _queueMappings[message.GetType()];

                await mapping.ScheduleMessageAsync(message, scheduleEnqueueTimeUtc);
            }
        }

        public Task ScheduleTopicMessageAsync<T>(T message, DateTimeOffset scheduleEnqueueTimeUtc) where T : new()
        {
            return ScheduleTopicMessagesAsync(new T[1]
            {
                message
            }, scheduleEnqueueTimeUtc);
        }

        public Task ScheduleTopicMessageAsync<T>(JsonSerializedMessage<T> message, DateTimeOffset scheduleEnqueueTimeUtc) where T : new()
        {
            return ScheduleTopicMessagesAsync(new JsonSerializedMessage<T>[1]
            {
                message
            }, scheduleEnqueueTimeUtc);
        }

        public async Task ScheduleTopicMessagesAsync<T>(IList<T> messageList, DateTimeOffset scheduleEnqueueTimeUtc) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType()).Distinct();

            EnsureTopicMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _topicMappings[message.GetType()];

                await mapping.ScheduleMessageAsync(new JsonSerializedMessage<T>(message), scheduleEnqueueTimeUtc);
            }
        }

        public async Task ScheduleTopicMessagesAsync<T>(IList<JsonSerializedMessage<T>> messageList, DateTimeOffset scheduleEnqueueTimeUtc) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType().GetGenericArguments()[0]).Distinct();
            EnsureTopicMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _topicMappings[message.GetType().GetGenericArguments()[0]];

                await mapping.ScheduleMessageAsync(message, scheduleEnqueueTimeUtc);
            }
        }

        public Task SendQueueMessageAsync<T>(T message) where T : new()
        {
            return SendQueueMessagesAsync(new T[1]
            {
                message
            });
        }

        public Task SendQueueMessageAsync<T>(JsonSerializedMessage<T> message) where T : new()
        {
            return SendQueueMessagesAsync(new JsonSerializedMessage<T>[1]
            {
                message
            });
        }

        public async Task SendQueueMessagesAsync<T>(IList<T> messageList) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType()).Distinct();

            EnsureQueueMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _queueMappings[message.GetType()];

                await mapping.SendAsync(new JsonSerializedMessage<T>(message));
            }
        }

        public async Task SendQueueMessagesAsync<T>(IList<JsonSerializedMessage<T>> messageList) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType()).Distinct();

            EnsureQueueMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _queueMappings[message.GetType()];

                await mapping.SendAsync(message);
            }
        }

        public Task SendTopicMessageAsync<T>(T message) where T : new()
        {
            return SendTopicMessagesAsync(new T[1]
            {
                message
            });
        }

        public Task SendTopicMessageAsync<T>(JsonSerializedMessage<T> message) where T : new()
        {
            return SendTopicMessagesAsync(new JsonSerializedMessage<T>[1]
            {
                message
            });
        }

        public async Task SendTopicMessagesAsync<T>(IList<T> messageList) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType()).Distinct();

            EnsureTopicMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _topicMappings[message.GetType()];

                await mapping.SendAsync(new JsonSerializedMessage<T>(message));
            }
        }

        public async Task SendTopicMessagesAsync<T>(IList<JsonSerializedMessage<T>> messageList) where T : new()
        {
            var messageTypes = messageList.Select(m => m.GetType().GetGenericArguments()[0]).Distinct();
            EnsureTopicMessagesAreMapped(messageTypes);

            foreach (var message in messageList)
            {
                var mapping = _topicMappings[message.GetType().GetGenericArguments()[0]];

                await mapping.SendAsync(message);
            }
        }

        internal void EnsureQueueMessagesAreMapped(IEnumerable<Type> messageTypes)
        {
            // Check that all of the message types have been registered first
            foreach (var messageType in messageTypes)
            {
                if (!_queueMappings.ContainsKey(messageType))
                {
                    throw new MessageNotMappedException($"There is no topic client registered for the message type {messageType}");
                }
            }
        }

        internal void EnsureTopicMessagesAreMapped(IEnumerable<Type> messageTypes)
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.ServiceBus;

using Turnoutt.Azure.ServiceBus.Core.Exceptions;
using Turnoutt.Azure.ServiceBus.Core.Messages;

namespace Turnoutt.Azure.ServiceBus.Core
{
    internal class ServiceBusClientPool : IServiceBusClientPool
    {
        private readonly IDictionary<Type, IQueueClient> _queueMappings;
        private readonly IDictionary<Type, ITopicClient> _topicMappings;

        public ServiceBusClientPool(ServiceBusClientPoolBuilder builder)
        {
            _topicMappings = builder.TopicMappings;
            _queueMappings = builder.QueueMappings;
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
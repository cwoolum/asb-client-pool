using System;
using System.Collections.ObjectModel;

using Microsoft.Azure.ServiceBus;

namespace Turnoutt.Azure.ServiceBus.Core
{
    public sealed class ServiceBusClientPoolBuilder
    {
        private readonly ServiceBusConnection _connection;
        private MessageEndpointQueueMap _queuePool = new MessageEndpointQueueMap();
        private MessageEndpointTopicMap _topicPool = new MessageEndpointTopicMap();

        public ServiceBusClientPoolBuilder(ServiceBusConnection connection)
        {
            _connection = connection;
        }

        public ServiceBusConnection Connection => _connection;

        internal ReadOnlyDictionary<Type, IQueueClient> QueueMappings => _queuePool.MessageEndpointMappings;

        internal ReadOnlyDictionary<Type, ITopicClient> TopicMappings => _topicPool.MessageEndpointMappings;

        public void AddQueueClient<T>(string entityPath, ReceiveMode receiveMode = ReceiveMode.PeekLock, RetryPolicy retryPolicy = null)
        {
            var messageTopicClient = new QueueClient(_connection, entityPath, receiveMode, retryPolicy);
            _queuePool.AddMapping(typeof(T), messageTopicClient);
        }

        public void AddQueueClient<T>(IQueueClient topicClient)
        {
            _queuePool.AddMapping(typeof(T), topicClient);
        }

        public void AddTopicClient<T>(string entityPath)
        {
            var messageTopicClient = new TopicClient(_connection, entityPath, RetryPolicy.Default);
            _topicPool.AddMapping(typeof(T), messageTopicClient);
        }

        public void AddTopicClient<T>(ITopicClient topicClient)
        {
            _topicPool.AddMapping(typeof(T), topicClient);
        }
    }
}
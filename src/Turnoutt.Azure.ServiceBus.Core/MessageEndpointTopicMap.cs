using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

using Microsoft.Azure.ServiceBus;

namespace Turnoutt.Azure.ServiceBus.Core
{
    internal class MessageEndpointTopicMap
    {
        private readonly ConcurrentDictionary<Type, ITopicClient> _messageEndpointMappings = new ConcurrentDictionary<Type, ITopicClient>();

        internal ReadOnlyDictionary<Type, ITopicClient> MessageEndpointMappings => new ReadOnlyDictionary<Type, ITopicClient>(_messageEndpointMappings);

        internal void AddMapping(Type messageType, ITopicClient client)
        {
            if (_messageEndpointMappings.ContainsKey(messageType))
            {
                _messageEndpointMappings[messageType] = client;
            }
            else
            {
                _messageEndpointMappings.TryAdd(messageType, client);
            }
        }
    }
}
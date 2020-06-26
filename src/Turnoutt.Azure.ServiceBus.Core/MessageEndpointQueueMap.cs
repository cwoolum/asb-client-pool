using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

using Microsoft.Azure.ServiceBus;

namespace Turnoutt.Azure.ServiceBus.Core
{
    internal class MessageEndpointQueueMap
    {
        private readonly ConcurrentDictionary<Type, IQueueClient> _messageEndpointMappings = new ConcurrentDictionary<Type, IQueueClient>();

        internal ReadOnlyDictionary<Type, IQueueClient> MessageEndpointMappings => new ReadOnlyDictionary<Type, IQueueClient>(_messageEndpointMappings);

        internal void AddMapping(Type messageType, IQueueClient client)
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
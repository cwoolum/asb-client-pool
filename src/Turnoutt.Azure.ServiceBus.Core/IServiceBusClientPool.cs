using System.Collections.Generic;
using System.Threading.Tasks;

namespace Turnoutt.Azure.ServiceBus.Core
{
    public interface IServiceBusClientPool
    {
        Task SendQueueMessageAsync<T>(IList<T> messageList) where T : new();

        Task SendQueueMessageAsync<T>(T message) where T : new();

        Task SendTopicMessageAsync<T>(IList<T> messageList) where T : new();

        Task SendTopicMessageAsync<T>(T message) where T : new();
    }
}
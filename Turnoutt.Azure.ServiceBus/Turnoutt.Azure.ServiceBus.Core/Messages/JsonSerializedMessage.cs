using System.Text.Json;

using Microsoft.Azure.ServiceBus;

namespace Turnoutt.Azure.ServiceBus.Core.Messages
{
    internal class JsonSerializedMessage<T> : Message
    {
        public JsonSerializedMessage(T messageContent) : base(JsonSerializer.SerializeToUtf8Bytes(messageContent))
        {
        }
    }
}
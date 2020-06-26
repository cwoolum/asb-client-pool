using System;

namespace Turnoutt.Azure.ServiceBus.Core.Exceptions
{
    public class MessageNotMappedException : Exception
    {
        public MessageNotMappedException(string message) : base(message)
        {
        }
    }
}
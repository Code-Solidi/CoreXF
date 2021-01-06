using System.Collections.Generic;

namespace EventBus.Interfaces
{
    public interface IChannel
    {
        IEnumerable<IMessage> Peek(string messageType);

        IEnumerable<string> MessageTypes { get; }
    }
}
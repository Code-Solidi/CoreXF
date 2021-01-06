using CoreXF.Messaging.Abstractions.Messages;

using System.Collections.Generic;

namespace CoreXF.Messaging.Abstractions.Channels
{
    public interface IFireAndForgetChannel //: IChannel
    {
        void Fire(IFireAndForgetMessage message, string timeToLive = null);

        IEnumerable<AbstractMessage> Peek(string messageType);
    }
}
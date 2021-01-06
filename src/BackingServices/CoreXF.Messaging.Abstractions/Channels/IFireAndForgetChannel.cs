using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions.Channels
{
    public interface IFireAndForgetChannel : IChannel
    {
        void Fire(IFireAndForgetMessage message, string timeToLive = null);
    }
}
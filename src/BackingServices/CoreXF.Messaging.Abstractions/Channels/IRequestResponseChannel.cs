using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions.Channels
{
    public interface IRequestResponseChannel //: IChannel
    {
        IMessageResponse Request(IRequestMessage message);
    }
}
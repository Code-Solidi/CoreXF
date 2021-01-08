using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions.Channels
{
    public interface IPublishSubscribeChannel //: IChannel
    {
        void Publish(IPublishedMessage message);
    }
}
using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions
{
    public delegate IMessageResponse ResponseEvent(IRequestResponseMessage message);

    public delegate void RecieveEvent(IPublishSubscribeMessage message, out IMessageResponse response);

    public delegate void PublishEvent(IPublishSubscribeMessage message);

    public delegate void FireEvent(IFireAndForgetMessage message);

    public delegate void ForgetEvent(IFireAndForgetMessage message);

    public delegate void RequestEvent(IRequestResponseMessage message, IMessageResponse response);
}
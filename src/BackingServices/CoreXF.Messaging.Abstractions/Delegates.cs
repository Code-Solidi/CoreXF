using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions
{
    //public delegate IMessageResponse ResponseEvent(IRequestResponseMessage message);

    public delegate void RecieveEvent(IPublishedMessage message, out IMessageResponse response);

    public delegate void PublishEvent(IPublishedMessage message);

    public delegate void FireEvent(IFireAndForgetMessage message);

    public delegate void ForgetEvent(IFireAndForgetMessage message);

    public delegate void ResponseEvent(IRequestMessage message, IMessageResponse response);
}
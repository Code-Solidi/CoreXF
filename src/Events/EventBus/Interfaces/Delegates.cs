using EventBus.Foundation.Messages;

namespace EventBus.Interfaces
{
    public delegate void PublishEvent(IPublishSubscribeMessage message);

    public delegate void FireEvent(IFireForgetMessage message);

    public delegate void ForgetEvent(IFireForgetMessage message);

    public delegate void RequestEvent(IRequestResponseMessage message, IMessageResponse response);
}
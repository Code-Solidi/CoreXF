namespace EventBus.Interfaces
{
    public delegate IMessageResponse ResponseEvent(IRequestResponseMessage message);

    public delegate void RecieveEvent(IPublishSubscribeMessage message, out IMessageResponse response);
}
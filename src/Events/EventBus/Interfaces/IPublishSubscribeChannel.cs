namespace EventBus.Interfaces
{
    public interface IPublishSubscribeChannel : IChannel
    {
        void Publish(IPublishSubscribeMessage message);
    }
}
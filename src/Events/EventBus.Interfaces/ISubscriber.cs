namespace EventBus.Interfaces
{
    public interface ISubscriber
    {
        string Identity { get; }

        event RecieveEvent OnRecieve;

        IMessageResponse Recieve(IPublishSubscribeMessage message);
    }
}
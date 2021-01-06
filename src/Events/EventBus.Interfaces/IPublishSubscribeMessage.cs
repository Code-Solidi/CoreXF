namespace EventBus.Interfaces
{
    public interface IPublishSubscribeMessage : IMessage
    {
        string Callback { get; set; }
    }
}
namespace EventBus.Interfaces
{
    public interface IRequestResponseChannel : IChannel
    {
        IMessageResponse Request(IRequestResponseMessage message);
    }
}
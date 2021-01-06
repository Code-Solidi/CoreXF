namespace EventBus.Interfaces
{
    public interface IFireForgetChannel : IChannel
    {
        void Fire(IFireForgetMessage message, string timeToLive = null);
    }
}
using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions
{
    public interface ISubscriber
    {
        string Identity { get; }

        void Recieve(IPublishedMessage message);
    }
}
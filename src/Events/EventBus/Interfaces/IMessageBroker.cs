using System.Collections.Generic;

namespace EventBus.Interfaces
{
    public interface IMessageBroker
    {
        IRecipient AddRecipient(IRecipient recipient);

        IRecipient FindRecipient(string identity);

        void Fire(IFireForgetMessage message);

        IEnumerable<ISubscriber> GetSubscribers(string messageType);

        IEnumerable<IMessage> Peek(string messageType);

        void Publish(IPublishSubscribeMessage message);

        void Register(string messageType);

        IRecipient RemoveRecipient(string identity);

        IMessageResponse Request(IRequestResponseMessage message);

        ISubscriber Subscribe(ISubscriber subscriber, string messageType);

        ISubscriber Unsubscribe(string identity, string messageType);
    }
}
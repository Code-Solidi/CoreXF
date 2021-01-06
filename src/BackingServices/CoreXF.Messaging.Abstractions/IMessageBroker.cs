
using CoreXF.Messaging.Abstractions.Messages;

using System.Collections.Generic;

namespace CoreXF.Messaging.Abstractions
{
    public interface IMessageBroker
    {
        void AddRecipient(string messageType, IRecipient recipient);
        
        bool FindRecipient(string messageType);

        void RemoveRecipient(string messageType);

        IMessageResponse Request(IRequestResponseMessage message);

        void Fire(IFireAndForgetMessage message);

        IEnumerable<ISubscriber> GetSubscribers(string messageType);

        IEnumerable<IFireAndForgetMessage> Peek(string messageType);

        void Publish(IPublishSubscribeMessage message);

        void Register(string messageType);

        bool IsRegistered(string messageType);

        void Subscribe(ISubscriber subscriber, string messageType);

        void Unsubscribe(string identity, string messageType);
    }
}
using CoreXF.Messaging.Abstractions.Messages;

using System.Collections.Generic;

namespace CoreXF.Messaging.Abstractions.Channels
{
    public interface IChannelFactory
    {
        IDictionary<string, ICollection<IMessage>> MessageQueue { get; }

        IEnumerable<IMessage> GetAllMessages();

        IFireAndForgetChannel CreateFireAndForgetChannel(IMessageBroker broker);

        IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker);

        IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker);
    }
}
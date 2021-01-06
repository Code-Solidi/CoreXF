using System.Collections.Generic;

namespace EventBus.Interfaces
{
    public interface IChannelFactory
    {
        IDictionary<string, ICollection<IMessage>> MessageQueue { get; }

        IEnumerable<IMessage> GetAllMessages();

        IFireForgetChannel InProcessCreateFireForgetChannel(IMessageBroker broker);

        IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker);

        IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker);
    }
}
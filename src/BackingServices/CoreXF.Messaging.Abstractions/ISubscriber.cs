using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions
{
    public interface ISubscriber
    {
        string Identity { get; }

        //event RecieveEvent OnRecieve;

        IMessageResponse Recieve(IPublishSubscribeMessage message);
    }
}
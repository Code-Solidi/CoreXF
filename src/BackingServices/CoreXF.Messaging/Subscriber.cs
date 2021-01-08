using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Messages;

namespace CoreXF.Messaging
{
    /// <summary>
    /// This class is registered with the event bus' publisher controller. Each microservice which needs to be notified about some event registers a
    /// subscriber with the messageType (usually typeof(&lt;MessageDescendedn&gt;).FullName) with the event bus. Once the microservice is finished with
    /// these event (Message) types it should unregister itself.
    /// </summary>
    public class Subscriber : ISubscriber
    {
        public string Identity { get; }

        //public event RecieveEvent OnRecieve;

        public Subscriber(string identity)
        {
            this.Identity = identity;
        }

        public virtual void Recieve(IPublishedMessage message)
        {
            //var response = MessageResponse.Default;
            //var uriPatched = new PublishSubscribeMessage(message);
            //this.OnRecieve?.Invoke(uriPatched, out response);
        }
    }
}
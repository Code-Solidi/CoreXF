using EventBus.Foundation.Messages;
using EventBus.Interfaces;
using Miscelaneous;

namespace EventBus.Foundation
{
    /// <summary>
    /// This class is registered with the event bus' publisher controller. Each microservice which needs to be notified about some event registers a
    /// subscriber with the messageType (usualy typeof(&lt;MessageDescendedn&gt;).FullName) with the event bus. Once the microservice is finished with
    /// these event (Message) types it should unregister itself.
    /// </summary>
    public sealed class Subscriber : ISubscriber
    {
        public string Identity { get; }

        public string Base64Identity => this.Identity.ToBase64();

        public event RecieveEvent OnRecieve;

        public Subscriber(string identity)
        {
            this.Identity = identity;
        }

        public IMessageResponse Recieve(IPublishSubscribeMessage message)
        {
            var response = MessageResponse.Default;
            var uriPatched = new PublishSubscribeMessage(message, this.Base64Identity);
            this.OnRecieve?.Invoke(uriPatched, out response);
            return response;
        }
    }
}
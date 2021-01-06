using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;

using Microsoft.Extensions.Logging;

using System;
using System.Linq;

namespace CoreXF.Messaging.Channels.InProcess
{
    public class InProcessPublishSubscriberChannel : AbstractChannel, IPublishSubscribeChannel
    {
        private readonly IMessageBroker broker;

        internal InProcessPublishSubscriberChannel(AbstractChannelFactory factory, IMessageBroker broker, ILogger logger)
            : base(factory, logger)
        {
            this.broker = broker;
        }

        public void Publish(IPublishSubscribeMessage message)
        {
            try
            {
                this.Logger.LogInformation($"Publishing event '{message.Id}' of type '{message.Type}'.");
                var subscribers = this.broker.GetSubscribers(message.Type);
                this.Logger.LogInformation($"Notifying {subscribers.Count()} subscribers.");
                foreach (var subscriber in subscribers)
                {
                    var result = subscriber.Recieve(message);

                    // todo: do something withe the result here - log, other?
                }
            }
            catch (Exception x)
            {
                this.Logger.LogError($"Error publishing event '{message.Id}' of type '{message.Type}'.", x);
                throw;
            }
        }
    }
}
using EventBus.Foundation.Messages;
using EventBus.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Foundation.Channels.WebApi
{
    public class WebApiFireForgetChannel : WebApiChannelBase, IFireForgetChannel
    {
        public WebApiFireForgetChannel(IChannelFactory factory, ILogger logger) : base(factory, logger)
        {
        }

        public async Task FireAsync(IFireForgetMessage message, string timeToLive)
        {
            if (message.TimeToLive == default(TimeSpan) || message.TimeToLive == TimeSpan.MinValue)
            {
                if (TimeSpan.TryParse(timeToLive, out var ttl) == false)
                {
                    ttl = FireForgetMessage.DefaultTimeToLive;
                }

                message.TimeToLive = ttl;
            }

            await base.SendAsync(message, this.EventBusUri);
        }

        public IEnumerable<FireForgetMessage> PollAll(string messageType)
        {
            var messages = base.Peek(messageType);
            return messages.Cast<FireForgetMessage>();
        }

        public FireForgetMessage Poll(Guid messageId)
        {
            if (messageId == Guid.Empty)
            {
                throw new InvalidOperationException("Polling a message with an empty GUID.");
            }

            foreach (var messageType in base.MessageTypes)
            {
                var messages = base.Peek(messageType);
                var result = messages.SingleOrDefault(x => x.Id == messageId);
                if (result != null)
                {
                    return result as FireForgetMessage;
                }
            }

            return null;
        }
    }
}
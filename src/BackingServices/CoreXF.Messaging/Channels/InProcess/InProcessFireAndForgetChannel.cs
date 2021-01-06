using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Channels;
using CoreXF.Messaging.Messages;

using Microsoft.Extensions.Logging;

using System;

namespace CoreXF.Messaging.Channels.InProcess
{
    public class InProcessFireAndForgetChannel : AbstractChannel, IFireAndForgetChannel
    {
        private readonly InProcessChannelFactory factory;

        public InProcessFireAndForgetChannel(InProcessChannelFactory factory, ILogger logger)
            : base(factory, logger)
        {
            this.factory = factory;
        }

        public void Fire(IFireAndForgetMessage message, string timeToLive = null)
        {
            if (message.TimeToLive == default || message.TimeToLive == TimeSpan.MinValue)
            {
                if (TimeSpan.TryParse(timeToLive, out var ttl) == false)
                {
                    ttl = FireAndForgetMessage.DefaultTimeToLive;
                }

                message.TimeToLive = ttl;
            }

            this.factory.AddMessage(message);
        }
    }
}
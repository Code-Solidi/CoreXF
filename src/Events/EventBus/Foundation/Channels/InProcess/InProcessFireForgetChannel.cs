using EventBus.Foundation.Messages;
using EventBus.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace EventBus.Foundation.Channels.InProcess
{
    public class InProcessFireForgetChannel : AbstractChannel, IFireForgetChannel
    {
        private readonly InProcessChannelFactory factory;

        public InProcessFireForgetChannel(InProcessChannelFactory factory, ILogger logger)
            : base(factory, logger)
        {
            this.factory = factory;
        }

        public void Fire(IFireForgetMessage message, string timeToLive = null)
        {
            if (message.TimeToLive == default(TimeSpan) || message.TimeToLive == TimeSpan.MinValue)
            {
                if (TimeSpan.TryParse(timeToLive, out var ttl) == false)
                {
                    ttl = FireForgetMessage.DefaultTimeToLive;
                }

                message.TimeToLive = ttl;
            }

            // nb: forced to become async, actualy just a sync method
            //await Task.Run(() => this.factory.AddMessage(message));
            this.factory.AddMessage(message);
        }
    }
}
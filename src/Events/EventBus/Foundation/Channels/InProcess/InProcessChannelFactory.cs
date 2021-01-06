using EventBus.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace EventBus.Foundation.Channels.InProcess
{
    public class InProcessChannelFactory : AbstractFactory
    {
        private const int defaultPeriod = 1000;
        private readonly Timer timer;

        public InProcessChannelFactory(ILoggerFactory loggerFactory, int period = InProcessChannelFactory.defaultPeriod)
            : base(loggerFactory?.CreateLogger<InProcessChannelFactory>())
        {
            if (period <= 0)
            {
                this.logger.LogWarning($"Non-positive period supplied: {period}, set to default ({InProcessChannelFactory.defaultPeriod}).");
                period = InProcessChannelFactory.defaultPeriod;
            }

            this.timer = new Timer(this.RemoveDeadMessages, null, period, period);
        }

        public override IFireForgetChannel InProcessCreateFireForgetChannel(IMessageBroker broker)
        {
            var channel = new InProcessFireForgetChannel(this, this.logger);
            return channel;
        }

        public override IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker)
        {
            var channel = new InProcessPublishSubscriberChannel(this, broker, this.logger);
            return channel;
        }

        public override IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker)
        {
            var channel = new InProcessRequestResponseChannel(this, broker, this.logger);
            return channel;
        }
    }
}
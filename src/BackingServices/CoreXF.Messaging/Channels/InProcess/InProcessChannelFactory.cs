using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;

using Microsoft.Extensions.Logging;

using System;
using System.Threading;

namespace CoreXF.Messaging.Channels.InProcess
{
    public class InProcessChannelFactory : AbstractChannelFactory
    {
        public const int DefaultPeriod = 1000;
        private readonly Timer timer;

        public InProcessChannelFactory(ILoggerFactory loggerFactory, int period = InProcessChannelFactory.DefaultPeriod)
            : base(loggerFactory?.CreateLogger<InProcessChannelFactory>())
        {
            if (period <= 0)
            {
                throw new ArgumentException($"Non-positive period supplied: {period}, set to default ({InProcessChannelFactory.DefaultPeriod}).", nameof(period));
            }

            this.timer = new Timer(this.RemoveDeadMessages, null, period, period);
        }

        internal override IFireAndForgetChannel CreateFireAndForgetChannel(IMessageBroker broker)
        {
            return new InProcessFireAndForgetChannel(this, this.Logger);
        }

        internal override IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker)
        {
            return new InProcessPublishSubscriberChannel(this, broker, this.Logger);
        }

        internal override IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker)
        {
            return new InProcessRequestResponseChannel(this, broker, this.Logger);
        }
    }
}
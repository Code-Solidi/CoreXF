using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;

using Microsoft.Extensions.Logging;

using System;

namespace CoreXF.Messaging.Channels.InProcess
{
    public class InProcessChannelFactory : AbstractChannelFactory
    {
        public const int DefaultPeriod = 1000;
        private readonly int period;

        public InProcessChannelFactory(ILoggerFactory loggerFactory, int period = InProcessChannelFactory.DefaultPeriod)
            : base(loggerFactory?.CreateLogger<InProcessChannelFactory>())
        {
            this.period = period > 0
                ? period
                : throw new ArgumentException($"Non-positive period supplied: {period}, set to default ({InProcessChannelFactory.DefaultPeriod}).", nameof(period));
        }

        public override IFireAndForgetChannel CreateFireAndForgetChannel(IMessageBroker broker) => new InProcessFireAndForgetChannel(this, this.period, this.Logger);

        public override IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker) => new InProcessPublishSubscriberChannel(this, broker, this.Logger);

        public override IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker) => new InProcessRequestResponseChannel(this, broker, this.Logger);
    }
}
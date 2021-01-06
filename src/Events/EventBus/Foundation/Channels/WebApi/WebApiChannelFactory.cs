using EventBus.Interfaces;
using Microsoft.Owin.Logging;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace EventBus.Foundation.Channels.WebApi
{
    public class WebApiChannelFactory : AbstractFactory
    {
        private const int defaultPeriod = 1000;

        private readonly string eventBusUri;

        public WebApiChannelFactory(ILogger logger, int period = WebApiChannelFactory.defaultPeriod) : base(logger)
        {
            if (period <= 0)
            {
                this.logger.WriteWarning($"Non-positive period supplied: {period}, set to default ({WebApiChannelFactory.defaultPeriod}).");
                period = WebApiChannelFactory.defaultPeriod;
            }

            this.eventBusUri = ConfigurationManager.AppSettings["eventBusUri"];// todo: make it constant
        }

        public override IFireForgetChannel InProcessCreateFireForgetChannel(IMessageBroker broker)
        {
            return new WebApiFireForgetChannel(this, this.logger);
        }

        public override IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker)
        {
            return new WebApiPublishSubscribeChannel(this, this.logger);
        }

        public override IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker)
        {
            return new WebApiRequestResponseChannel(this, this.logger);
        }
    }
}
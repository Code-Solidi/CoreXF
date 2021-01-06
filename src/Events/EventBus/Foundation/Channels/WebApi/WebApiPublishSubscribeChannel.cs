using EventBus.Interfaces;
using Microsoft.Owin.Logging;

namespace EventBus.Foundation.Channels.WebApi
{
    public class WebApiPublishSubscribeChannel : WebApiChannelBase, IPublishSubscribeChannel
    {
        public WebApiPublishSubscribeChannel(IChannelFactory factory, ILogger logger) : base(factory, logger)
        {
        }

        public void Publish(IPublishSubscribeMessage message)
        {
            base.SendAsync(message, this.EventBusUri).GetAwaiter().GetResult();
        }
    }
}
using EventBus.Foundation.Messages;
using EventBus.Interfaces;
using Microsoft.Owin.Logging;

namespace EventBus.Foundation.Channels.WebApi
{
    public class WebApiRequestResponseChannel : WebApiChannelBase, IRequestResponseChannel
    {
        public WebApiRequestResponseChannel(IChannelFactory factory, ILogger logger) : base(factory, logger)
        {
        }

        public MessageResponse Request(IRequestResponseMessage message)
        {
            var result = base.SendAsync(message, this.EventBusUri).GetAwaiter().GetResult();
            return new MessageResponse(result);
        }
    }
}
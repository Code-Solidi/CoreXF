using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Messages;

using Microsoft.Extensions.Logging;

namespace CoreXF.Messaging.Channels.WebApi
{
    public class WebApiRequestResponseChannel : WebApiChannelBase, IRequestResponseChannel
    {
        public WebApiRequestResponseChannel(AbstractChannelFactory factory, ILogger logger) : base(factory, logger)
        {
        }

        public IMessageResponse Request(IRequestResponseMessage message)
        {
            var result = base.SendAsync(message, this.EventBusUri).GetAwaiter().GetResult();
            return new MessageResponse(result);
        }
    }
}
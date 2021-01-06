using EventBus.Foundation.Messages;
using EventBus.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;

namespace EventBus.Foundation.Channels.InProcess
{
    public class InProcessRequestResponseChannel : AbstractChannel, IRequestResponseChannel
    {
        private readonly IMessageBroker broker;
        private readonly ILogger logger;

        public InProcessRequestResponseChannel(IChannelFactory factory, IMessageBroker broker, ILogger logger)
            : base(factory, logger)
        {
            this.broker = broker;
            this.logger = logger;
        }

        public IMessageResponse Request(IRequestResponseMessage message)
        {
            var recipient = this.broker.FindRecipient(message.Recipient);
            var response = recipient?.Recieve(message);
            return response ?? new MessageResponse(new HttpResponseMessage
            {
                ReasonPhrase = "No response, is the recipient non-null entity? Or, did the recipient supply message handler?",
                StatusCode = HttpStatusCode.InternalServerError
            });
        }
    }
}
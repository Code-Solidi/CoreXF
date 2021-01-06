using CoreXF.Messaging.Abstractions;

using System.Net.Http;

namespace CoreXF.Messaging.Messages
{
    public class MessageResponse : IMessageResponse
    {
        public object Content { get; private set; }

        public StatusCode StatusCode { get; private set; }

        public IRecipient Recipient { get; }

        public string ReasonPhrase { get; private set; }

        public static IMessageResponse Default = new MessageResponse(default, StatusCode.Success);

        public MessageResponse(IRecipient recipient, StatusCode statusCode, string reasonPhrase = default)
        {
            this.Recipient = recipient;
            this.StatusCode = statusCode;
            this.ReasonPhrase = reasonPhrase;
        }

        public MessageResponse(HttpResponseMessage result)
        {
        }

        public void SetContent(object content)
        {
            this.Content = content;
        }
    }
}
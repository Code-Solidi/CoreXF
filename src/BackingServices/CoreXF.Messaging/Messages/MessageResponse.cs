using CoreXF.Messaging.Abstractions;

namespace CoreXF.Messaging.Messages
{
    public class MessageResponse : IMessageResponse
    {
        public object Content { get; private set; }

        public StatusCode StatusCode { get; private set; }

        public IRecipient Recipient { get; private set; }

        public string ReasonPhrase { get; private set; }

        public static IMessageResponse Default = new MessageResponse(default, StatusCode.Success);

        public MessageResponse(IRecipient recipient, StatusCode statusCode, string reasonPhrase = default)
        {
            this.Recipient = recipient;
            this.StatusCode = statusCode;
            this.ReasonPhrase = reasonPhrase;
        }

        public void SetContent(object content)
        {
            this.Content = content;
        }

        public void SetRecipient(IRecipient recipient)
        {
            this.Recipient = recipient;
        }
    }
}
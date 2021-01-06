using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Messages;

namespace CoreXF.Messaging
{
    public class Recipient : IRecipient
    {
        public string Identity { get; set; }

        public string Base64Identity => this.Identity.ToBase64();

        public virtual IMessageResponse Recieve(IRequestResponseMessage message)
        {
            return MessageResponse.Default;
        }
    }
}
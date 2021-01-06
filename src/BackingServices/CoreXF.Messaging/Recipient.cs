using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Messages;

namespace CoreXF.Messaging
{
    public class Recipient : IRecipient
    {
        public string Identity { get; }

        public Recipient(string identity)
        {
            this.Identity = identity;
        }

        public virtual IMessageResponse Recieve(IRequestResponseMessage message)
        {
            return MessageResponse.Default;
        }
    }
}
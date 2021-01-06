using EventBus.Foundation.Messages;
using EventBus.Interfaces;
using Miscelaneous;

namespace EventBus.Foundation
{
    public class Recipient : IRecipient
    {
        public string Identity { get; set; }

        public string Callback { get; set; }

        public string Base64Identity => this.Identity.ToBase64();

        public event ResponseEvent OnRecieve;

        public IMessageResponse Recieve(IRequestResponseMessage message)
        {
            var response = MessageResponse.Default;
            if (string.IsNullOrWhiteSpace(this.Callback) == false)
            {
                message.Callback = this.Callback.Replace("/id/", $"/{this.Base64Identity}/");
                response = this.OnRecieve?.Invoke(message);
            }

            return response;
        }
    }
}
using EventBus.Interfaces;

namespace EventBus.Foundation.Messages
{
    public class PublishSubscribeMessage : AbstractMessage, IPublishSubscribeMessage
    {
        public PublishSubscribeMessage()
        {
        }

        public PublishSubscribeMessage(IPublishSubscribeMessage message, string subscriberId)
        {
            this.Callback = message.Callback.Replace("/id/", $"/{subscriberId}/");
            this.DateTime = message.DateTime;
            this.Payload = message.Payload;
            this.Type = message.Type;
        }

        public string Callback { get; set; }
    }
}
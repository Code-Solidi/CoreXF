using EventBus.Foundation;
using EventBus.Foundation.Messages;
using EventBus.Interfaces;
using Miscelaneous;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using static Miscelaneous.Utility;

namespace EventBus.Client
{
    public class EventBusClient : IEventBus
    {
        private readonly string uri;

        public EventBusClient(string uri)
        {
            this.uri = Path.Combine(uri, "api/events/");
        }

        public HttpResponseMessage AddRecipient(string identity, string callback = null)
        {
            var recipient = new Recipient { Identity = identity, Callback = callback ?? string.Empty };
            var content = JsonConvert.SerializeObject(recipient);
            var requestUri = Path.Combine(this.uri, "addrecipient");
            return Utility.SendAsync(requestUri, content).GetAwaiter().GetResult();
        }

        public HttpResponseMessage Fire(string type, string payload, TimeSpan timeToLive)
        {
            var content = JsonConvert.SerializeObject(new FireForgetMessage
            {
                Type = type,
                Payload = payload,
                TimeToLive = timeToLive
            });

            return Utility.SendAsync(Path.Combine(this.uri, "fire"), content).GetAwaiter().GetResult();
        }

        public HttpResponseMessage Peek(string messageType)
        {
            var requestUri = Path.Combine(Path.Combine(this.uri, "peek"), messageType);
            return Utility.SendAsync(requestUri, string.Empty, Method.GET).GetAwaiter().GetResult();
        }

        public HttpResponseMessage Publish(string type, string payload, string callback)
        {
            var content = JsonConvert.SerializeObject(new PublishSubscribeMessage
            {
                Type = type,
                Payload = payload,
                Callback = callback
            });

            return Utility.SendAsync(Path.Combine(this.uri, "publish"), content).GetAwaiter().GetResult();
        }

        public HttpResponseMessage Register(string messageType)
        {
            var requestUri = Path.Combine(Path.Combine(this.uri, "register"), messageType);
            return Utility.SendAsync(requestUri, string.Empty, Method.GET).GetAwaiter().GetResult();
        }

        public HttpResponseMessage RemoveRecipient(string identity)
        {
            var requestUri = Path.Combine(this.uri, "removerecipient");
            return Utility.SendAsync(requestUri, identity).GetAwaiter().GetResult();
        }

        public HttpResponseMessage Request(string type, string payload, string recipient/*, string sender*/)
        {
            var content = JsonConvert.SerializeObject(new RequestResponseMessage(recipient)
            {
                Payload = payload,
                Type = type
            });

            return Utility.SendAsync(Path.Combine(this.uri, "request"), content).GetAwaiter().GetResult();
        }

        public HttpResponseMessage Subscribe(string subscriberIdentity, string messageType)
        {
            var content = JsonConvert.SerializeObject(new Subscriber(subscriberIdentity));
            var requestUri = Path.Combine(Path.Combine(this.uri, "subscribe"), messageType);
            return Utility.SendAsync(requestUri, content).GetAwaiter().GetResult();
        }

        public HttpResponseMessage Unsubscribe(string subscriberIdentity, string messageType)
        {
            var content = JsonConvert.SerializeObject(new Subscriber(subscriberIdentity));
            var requestUri = Path.Combine(Path.Combine(this.uri, "unsubscribe"), messageType);
            return Utility.SendAsync(requestUri, content).GetAwaiter().GetResult();
        }
    }
}
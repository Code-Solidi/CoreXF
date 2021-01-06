using System;
using System.Net.Http;

namespace EventBus.Interfaces
{
    public interface IEventBus
    {
        HttpResponseMessage Fire(string type, string payload, TimeSpan timeToLive);

        HttpResponseMessage Peek(string messageType);

        HttpResponseMessage Publish(string type, string payload, string callback);

        HttpResponseMessage Register(string messageType);

        HttpResponseMessage Request(string type, string payload, string recipient);

        HttpResponseMessage Subscribe(string subscriber, string messageType);

        HttpResponseMessage Unsubscribe(string subscriber, string messageType);

        HttpResponseMessage AddRecipient(string identity, string callback = null);

        HttpResponseMessage RemoveRecipient(string identity);
    }
}
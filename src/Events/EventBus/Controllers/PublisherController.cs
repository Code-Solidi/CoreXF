using EventBus.Foundation;
using EventBus.Foundation.Messages;
using EventBus.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace EventBus.Controllers
{
    public class PublisherController : ControllerBase
    {
        private readonly IMessageBroker broker;
        private readonly ILogger logger;

        public PublisherController(IMessageBroker broker, ILogger logger)
        {
            this.broker = broker;
            this.logger = logger;
        }

        [HttpPost("api/events/addrecipient")]
        public ActionResult AddRecipient([FromBody]Recipient recipient)
        {
            var added = this.broker.AddRecipient(recipient);
            if (added != null)
            {
                added.OnRecieve += this.RecipientOnRecieve;
                return this.Ok(added);
            }

            return this.UnprocessableEntity(recipient);

        }

        [HttpPost("api/events/fire")]
        public ActionResult FireEvent([FromBody]FireForgetMessage message)
        {
            this.logger.LogInformation($"Event of '{message.Type}' fired: {message.Payload}");
            this.broker.Fire(message);
            return this.Ok(message);
        }

        [HttpGet("api/events/peek/{messageType}")]
        public ActionResult PeekEvent(string messageType)
        {
            var result = this.broker.Peek(messageType);
            this.logger.LogInformation($"Events of '{messageType}' peeked: {result.Count()}");
            return this.Ok(result);
        }

        [HttpPost("api/events/publish")]
        public ActionResult PublishEvent([FromBody]PublishSubscribeMessage message)
        {
            this.logger.LogInformation($"Event of '{message.Type}' published: {message.Payload}");
            this.broker.Publish(message);
            return this.Ok(message);
        }

        [HttpPost("api/events/removerecipient")]
        public ActionResult RemoveRecipient([FromBody]Recipient recipient)
        {
            var removed = this.broker.RemoveRecipient(recipient.Identity);
            if (removed != null)
            {
                removed.OnRecieve -= this.RecipientOnRecieve;
                return this.Ok(removed);
            }

            return this.NotFound();
        }

        [HttpPost("api/events/request")]
        public ActionResult RequestEvent([FromBody]RequestResponseMessage message)
        {
            this.logger.LogInformation($"Event '{message.Type}:{message.Id}' request: {message.Payload}");
            var found = this.broker.FindRecipient(message.Recipient);
            if (found != null)
            {
                var result = this.broker.Request(message);
                var content = Encoding.Default.GetString(result.Content ?? new byte[0]);
                content = string.IsNullOrWhiteSpace(content) ? result.Response.ReasonPhrase : content;
                this.logger.LogInformation($"Event '{message.Type}:{message.Id}' response: '{content}'.");
                return this.StatusCode((int)result.Response.StatusCode, result.Response.ReasonPhrase);
            }

            this.logger.LogWarning($"Recipient of event '{message.Type}:{message.Id}' could not be found.");
            return this.NotFound();
        }

        [HttpPost("api/events/subscribe/{messageType}")]
        public ActionResult Subscribe(string messageType, [FromBody]Subscriber subscriber)
        {
            this.broker.Register(messageType);
            var subscribed = this.broker.Subscribe(subscriber, messageType);
            if (subscribed != null)
            {
                subscribed.OnRecieve += this.SubscriberOnRecieve;
                return this.Ok(subscribed);
            }

            return this.UnprocessableEntity(subscriber);
        }

        [HttpPost("api/events/unsubscribe/{messageType}")]
        public ActionResult Unsubscribe(string messageType, [FromBody]Subscriber subscriber)
        {
            var unsubscribed = this.broker.Unsubscribe(subscriber.Identity, messageType);
            if (unsubscribed != null)
            {
                unsubscribed.OnRecieve -= this.SubscriberOnRecieve;
                return this.Ok(unsubscribed);
            }

            return this.NotFound();
        }

        private IMessageResponse RecipientOnRecieve(IRequestResponseMessage message)
        {
            using (var client = new HttpClient())
            {
                var result = new HttpResponseMessage(HttpStatusCode.NoContent);
                try
                {
                    var content = new StringContent(JsonConvert.SerializeObject(message)
                        , Encoding.UTF8, "application/json");
                    result = client.PostAsync(message.Callback, content).GetAwaiter().GetResult();
                }
                catch (HttpRequestException x)
                {
                    result = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        ReasonPhrase = x.InnerException?.InnerException?.Message
                            ?? x.InnerException?.Message
                            ?? x.Message
                    };
                }

                var response = new MessageResponse(result);
                return response;
            }
        }

        private void SubscriberOnRecieve(IPublishSubscribeMessage message, out IMessageResponse response)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   // ACCEPT header

                var result = default(HttpResponseMessage);
                if (string.IsNullOrWhiteSpace(message.Callback) == false)
                {
                    try
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, message.Callback)
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json")
                        };

                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        this.logger.LogInformation($"Publishing: '{request.Content.ReadAsStringAsync().Result}'");
                        result = client.SendAsync(request).GetAwaiter().GetResult();
                    }
                    catch (HttpRequestException x)
                    {
                        result = new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = x.InnerException?.Message ?? x.Message };
                    }
                }

                response = new MessageResponse(result);
            }
        }
    }
}
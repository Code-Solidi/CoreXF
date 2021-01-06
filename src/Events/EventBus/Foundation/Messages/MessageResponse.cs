using EventBus.Interfaces;
using System.Net;
using System.Net.Http;

namespace EventBus.Foundation.Messages
{
    public class MessageResponse : IMessageResponse
    {
        public HttpResponseMessage Response { get; set; }

        public static IMessageResponse Default = new MessageResponse { Response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK } };

        public MessageResponse()
        {
        }

        public MessageResponse(HttpResponseMessage result)
        {
            this.Response = result;
            if (this.Response.IsSuccessStatusCode)
            {
                this.Content = result.Content?.ReadAsByteArrayAsync().Result;
            }
        }

        public byte[] Content { get; }
    }
}
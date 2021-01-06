using EventBus.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Foundation.Channels.WebApi
{
    public class WebApiChannelBase : AbstractChannel
    {
        internal string EventBusUri { get; set; }

        public WebApiChannelBase(IChannelFactory factory, ILogger logger) : base(factory, logger)
        {
        }

        /// <summary>
        /// Sends the message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        protected virtual async Task<HttpResponseMessage> SendAsync(IMessage message, string uri)
        {
            using (var client = new HttpClient())
            {
                var response = default(HttpResponseMessage);
                var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                try
                {
                    response = await client.PostAsync(uri, content);
                }
                catch (HttpRequestException x)
                {
                    if (response != null)
                    {
                        this.Logger.LogError($"Event buss error: {response.StatusCode} ('{response.ReasonPhrase}').", x);
                    }
                }
                catch (Exception x)
                {
                    this.Logger.LogError($"Event buss error.", x);
                }

                return response;
            }
        }
    }
}
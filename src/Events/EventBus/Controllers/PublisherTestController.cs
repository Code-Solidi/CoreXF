using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

namespace EventBus.Controllers
{
    public class PublisherTestController : ControllerBase
    {
        [HttpPost("api/events/subscription/{data}")]
        public ActionResult Callback(string data)
        {
            return this.Ok(data);
        }

        [HttpPost("api/events/subscription/{id}/{data}")]
        public ActionResult Subscription(string id, string data)
        {
            var identity = this.FromBase64(id);
            return this.Ok(new { identity, data });
        }

        [HttpPost("api/events/requests/{id}/{data}")]
        public ActionResult Requests(string id, string data)
        {
            var identity = this.FromBase64(id);
            return this.Ok(new { identity, data });
        }

        private string FromBase64(string value)
        {
            var decodedCharArray = Convert.FromBase64String(value);
            var result = Encoding.UTF8.GetString(decodedCharArray);
            return result;
        }
    }
}
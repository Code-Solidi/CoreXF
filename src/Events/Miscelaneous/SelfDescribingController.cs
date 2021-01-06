using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Miscelaneous
{
    public class SelfDescribingController : ControllerBase
    {
        protected ActionResult Describe()
        {
            if (DescribeHelper.Inspect(this.Request.Headers))
            {
                return this.RedirectToAction(nameof(SelfDescribingController.Query));
            }

            return null;
        }

        [HttpGet("/api/[controller]/help")]
        public ActionResult Query()
        {
            var description = DescribeHelper.Describe(this);
            var result = JsonConvert.DeserializeObject(description);
            return this.Ok(result);
        }
    }
}
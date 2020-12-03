using CoreXF.Abstractions.Attributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAplExt2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Export]
    public class CalculatorController : ControllerBase
    {
        private readonly ILogger<CalculatorController> _logger;

        public CalculatorController(ILogger<CalculatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public int Get(int a, int b)
        {
            return a + b;
        }
    }
}
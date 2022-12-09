/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAplExt2.Controllers
{
    [ApiController, Route("[controller]")]
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
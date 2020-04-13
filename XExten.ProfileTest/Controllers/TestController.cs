using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace XExten.ProfileTest.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class TestController : ControllerBase
    {
        private static readonly DiagnosticSource testDiagnosticListener = new DiagnosticListener("HttpTest");
        [HttpGet("Get")]
        public List<string> Get()
        {
            if (testDiagnosticListener.IsEnabled("HttpTests"))
            {
                testDiagnosticListener.Write("HttpTests", "hello world");
            }

            return new List<string>();
        }
    }
}

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
        //private static readonly DiagnosticSource testDiagnosticListener = new DiagnosticListener("Microsoft.AspNetCore");
        [HttpGet("Get")]
        public List<string> Get()
        {
            //if (testDiagnosticListener.IsEnabled("Microsoft.AspNetCore.Hosting.BeginRequest"))
            //{
            //    testDiagnosticListener.Write("Microsoft.AspNetCore.Hosting.BeginRequest", HttpContext);
            //}

            return new List<string>();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XExten.ProfileTest.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class TraceController: ControllerBase
    {
        [HttpPost("SetTrace")]
        public string SetTrace(dynamic data)
        {
            var xx = data;
            return "1";
        }
    }
}

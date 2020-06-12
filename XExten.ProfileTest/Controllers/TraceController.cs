using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XExten.XCore;

namespace XExten.ProfileTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TraceController: ControllerBase
    {
        [HttpPost("SetTrace")]
        public string SetTrace(Object data)
        {
            var xx = data.ToString().ToModel<JObject>();
            return "1";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XExten.HttpFactory;
using Microsoft.Extensions.Logging;
using System.Reflection;
using XExten.Profile.AspNetCore.Source;
using XExten.Profile.AspNetCore.InvokeTracing;

namespace XExten.ProfileTest.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("Get")]
        public List<string> Get()
        {
            TestClass tc = new TestClass();
            var xx = tc.GetType().GetMethod("TestMethod").ByTraceInvoke(tc, new object[] { "123a", 111 });


            //tc.TestMethod("a", 11);

            //HttpMultiClient.HttpMulti.AddNode("https://www.baidu.com").Build().RunString();

            // var xx = Sugar.DB.Queryable<WarnInfo>().Select(t => new WarnInfo
            // {
            //     Title = t.Title,
            //     ZhaiYao = t.ZhaiYao
            // }).ToList();

            //var x =  xx.FirstOrDefault();
            // x.Title = "测试1";
            // Sugar.DB.Updateable(x).UpdateColumns(t => t.Title).Where(t => t.ZhaiYao == "111").ExecuteCommand();
            return new List<string>();
        }
    }

    public class TestClass
    {
        public async Task<int> TestMethod(string name, int index)
        {
            Convert.ToInt32(name);
            Convert.ToString(index);
            return await Task.FromResult(123);
        }

        public  int TestMethodNo(string name, int index)
        {
            Convert.ToInt32(name);
            Convert.ToString(index);
            return 123;
        }
    }
}

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
using XExten.Common;
using Microsoft.EntityFrameworkCore;

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
            var data = ResultProvider.SetValue("Name", new Dictionary<object, object> { { "Key", "Value" } });
            tc.ByTraceInvoke("TestMethods", new object[] { data });

            //HttpMultiClient.HttpMulti.AddNode("https://www.baidu.com").Build().RunString();

            var xx = Sugar.DB.Queryable<WarnInfo>().Select(t => new WarnInfo
            {
                Title = t.Title,
                ZhaiYao = t.ZhaiYao
            }).ToList();

            var x =  xx.FirstOrDefault();
            // x.Title = "测试1";
            // Sugar.DB.Updateable(x).UpdateColumns(t => t.Title).Where(t => t.ZhaiYao == "111").ExecuteCommand();
            return new List<string>();
        }

        [HttpGet("Gets")]
        public string Gets() {
            SugarContext db = new SugarContext();
           var data =  db.warnInfos.FromSqlRaw("select Title,ZhaiYao from WarnInfo").FirstOrDefault();
            return "123";
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

        public  int TestMethods(ResultProvider provider)
        {
            return 111;
        }
    }
}

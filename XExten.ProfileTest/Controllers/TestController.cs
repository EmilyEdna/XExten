using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XExten.HttpFactory;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
            TestClass tc = new TestClass();
            tc.TestMethod("a", 11);

            //if (testDiagnosticListener.IsEnabled("Microsoft.AspNetCore.Hosting.BeginRequest"))
            //{
            //    testDiagnosticListener.Write("Microsoft.AspNetCore.Hosting.BeginRequest", HttpContext);
            //}

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

        private static readonly DiagnosticSource testDiagnosticListener = new DiagnosticListener("MethodHandlerDiagnosticListener");

        public void TestMethod(string name, int index)
        {
            if (testDiagnosticListener.IsEnabled("ExecuteCommandMethodStar"))
            {
                testDiagnosticListener.Write("ExecuteCommandMethodStar", );
            }
            XPlus.XPlusEx.XTry(() =>
            {
                Convert.ToInt32(name);
                Convert.ToString(index);
                if (testDiagnosticListener.IsEnabled("ExecuteCommandMethodEnd"))
                {
                    testDiagnosticListener.Write("ExecuteCommandMethodEnd", "time");
                }
            }, ex =>
            {
                if (testDiagnosticListener.IsEnabled("ExecuteCommandMethodException"))
                {
                    testDiagnosticListener.Write("ExecuteCommandMethodException", ex);
                }
            });



        }
        public void TestMethod2(string xxx)
        {

        }
    }
}

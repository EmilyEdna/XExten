#### 注意！使用该包需配合服务端XExten.SocketProxyServer使用
[![](https://img.shields.io/badge/build-success-brightgreen.svg)](https://github.com/EmilyEdna/XExten.SocketProxy)
[![](https://img.shields.io/badge/nuget-v1.0.5.1-blue.svg)](https://www.nuget.org/packages/XExten.SocketProxy/1.0.5.1)
## 使用方法
------------------------------------
1.在一个控制器中定义方法如下：
``` c#
    [SocketRoute("OtherApi", ControllerName = "Other")]
    public class OtherController : Controller
    {
        [HttpGet]
        [SocketMethod]
        public async Task<ActionResult<object>> Test1()
        {
            return await Task.FromResult("123");
        }
    }
```
2.初始化如下:
``` c#
     SocketBasic.InitInternalSocket(option =>
     {
         option.SockInfoIP = "中间件IP";
         option.SockInfoPort = "中间件Port";
         option.ClientPath = "本机IP";
         option.ClientPort ="本机链接中间件的固定Port";
         option.CallHandle="自定义处理类";
     }, true);
```
3.调用如下:
``` c#
    [Route("Api/[controller]/[action]")]
    [ApiController]
    public class SocketTestController : Controller
    {
        [HttpGet]
        public async Task<ActionResult<string>> SocketTest() 
        {
            SocketSerializeData SSD = new SocketSerializeData(); 
            SSD.AppendRoute("TestApi/other/GetTest")
                .AppendSerialized("Name", "lzh")
                .AppendSerialized(new Dictionary<string, object> { { "Age", 26 } })
                .AppendSerialized(new { Card = 100 });
            Call.SendInternalInfo(SSD);
            var data = CallEventAction.Instance().DelegateResult;


            SocketSerializeData SSD1 = new SocketSerializeData(); ;
            SSD1.AppendRoute("TestApi/other/GetTest1").AppendSerialized("Name", "lzh");
            Call.SendInternalInfo(SSD1, new SocketSessionDefault
            {
                SessionAccount = "admin",
                CustomizeData = "aa",
                PrimaryKey = "123",
                SessionRole = "Admin"
            });
            var data1 = CallEventAction.Instance().DelegateResult;

            return await Task.FromResult("123");
        }
    }
```
4.自定义实现处理:
```c#
    public class CallHandle: CallHandleAbstract
    {
        /// <summary>
        /// Analysis function
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public override ISocketResult ExecuteCallFuncHandler(SocketMiddleData Param)
        {
          return base.ExecuteCallFuncHandler(Param);
        }
        /// <summary>
        /// Execute Session
        /// </summary>
        /// <param name="Controller"></param>
        /// <param name="Method"></param>
        protected override bool ExecuteCallSessionHandler(MethodInfo Method, ISocketSession Session)
        {
            return base.ExecuteCallSessionHandler(Method, Session);
        }
        /// <summary>
        /// Execute handler data
        /// </summary>
        /// <param name="Controller"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
        protected override ISocketResult ExecuteCallDataHandler(Type Controller, MethodInfo Method, ISocketResult Param)
        {
            return base.ExecuteCallDataHandler(Controller, Method, Param);
        }
```
5.实现Session验证:
```c#
    public interface ISocketSessionHandler
    {
        bool Executing(ISocketSession Session);
    }
```
6.处理方式如下:
```c#
    [Route("Api/[controller]/[action]")]
    [ApiController]
    [SocketRoute("TestApi")]
    public class OtherController : Controller
    {
        [SocketMethod]
        [SocketAuthor(false)]
        public async Task<ActionResult<Object>> GetTest(ResultProvider Param)
        {
            return await Task.FromResult("我是测测试");
        }


        [SocketMethod]
        [SocketAuthor(true)]
        public async Task<ActionResult<Object>> GetTest1(ResultProvider Param)
        {
            return await Task.FromResult("我是测测试");
        }
    }

    public class SocketSessionHandler : ISocketSessionHandler
    {
        public bool Executing(ISocketSession Session)
        {
            var ss = Session;
            return false;
        }
    }
```

#### 注意！使用该包需配合服务端XExten.SocketProxyServer使用
[![](https://img.shields.io/badge/build-success-brightgreen.svg)](https://github.com/EmilyEdna/XExten.SocketProxy)
[![](https://img.shields.io/badge/nuget-v1.0.3-blue.svg)](https://www.nuget.org/packages/XExten.SocketProxy/1.0.3)
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
    public class MainController : Controller
    {
        [HttpGet]
        public async Task<ActionResult<object>> Test2()
        {
            SocketSerializeData SSD = new SocketSerializeData(); ;
            SSD.AppendRoute("otherapi/other/test1");
            CallEvent.SendInternalInfo(SSD);
            var data = CallHandleEventAction.Instance().DelegateResult;
            //...
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
5.实现Session验证
```c#
    public interface ISocketSessionHandler
    {
        bool Executing(ISocketSession Session);
    }
```

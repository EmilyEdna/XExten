#### 为分布式系统提供的简单的系统追踪组件
[![](https://img.shields.io/badge/build-success-brightgreen.svg)](https://github.com/EmilyEdna/XExten.Profile)
[![](https://img.shields.io/badge/nuget-v1.0.0-blue.svg)](https://www.nuget.org/packages/XExten.Profile/1.0.0)
## 前言
------------------------------------
目前功能支持SQLServer,EFCore,AspNetCore,HttpClient,Method的跟踪，相对前面4个而言使用上只需要在程序Startup中注入即可，而Method则不然。
## 使用方法
------------------------------------
1.在AspNetCore中Startup中里面注册APM
``` c#
    services.RegistXExtenService();
```
2.配套使用UI界面同样在Startup注册:
``` c#
     app.UseTraceUI();
```
3.Http只要调用System.Net相关就会自动执行例如:
``` c#
    HttpMultiClient.HttpMulti.AddNode("https://www.baidu.com").Build().RunString();
```
4.SQL类只要执行SQL相关就会自动执行例如:
```c#
   (new DbContext()).warnInfos.FirstOrDefault();
```
5.对于要实现Method的跟踪有两种模式第一种通过方式一实现，第二种是自定义proxy实现:
```c#
    //方式一
    TestClass tc = new TestClass();
    var data = ResultProvider.SetValue("Name", new Dictionary<object, object> { { "Key", "Value" } });
    tc.ByTraceInvoke("TestMethods", new object[] { data });
    //方式二是根据客户自己自定义的方式实现这里不作演示
```
## UI下载地址
https://github.com/EmilyEdna/XExten/releases

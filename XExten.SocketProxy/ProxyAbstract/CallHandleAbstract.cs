using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XExten.Common;
using XExten.SocketProxy.SocketConfig;
using XExten.SocketProxy.SocketDependency;
using XExten.SocketProxy.SocketInterface;
using XExten.SocketProxy.SocketInterface.DefaultImpl;
using XExten.SocketProxy.SocketRoute;
using XExten.XCore;

namespace XExten.SocketProxy.ProxyAbstract
{
    public abstract class CallHandleAbstract
    {
        /// <summary>
        /// 逆向解析方法
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public  virtual ISocketResult ExecuteCallFuncHandler(SocketMiddleData Param)
        {
            //一定是其它服务
            List<Type> SourceTypes = DependencyLibrary.Dependency.Where(item => item.GetCustomAttribute(typeof(SocketRouteAttribute)) != null).ToList();
            var Routes = Param.MiddleResult.Router.Split("/").ToList();//接受的数据路由
            foreach (var Items in SourceTypes)
            {
                SocketRouteAttribute SocketRoute = (Items.GetCustomAttribute(typeof(SocketRouteAttribute)) as SocketRouteAttribute);
                //比较接受的路由和当前程序的路由是否一直
                //全部得小写
                if (SocketRoute.InternalServer.ToLower() == Routes[0] && (SocketRoute.ControllerName.IsNullOrEmpty() ? Items.Name.ToLower().Contains(Routes[1]) : Items.Name.ToLower().Contains(SocketRoute.ControllerName.ToLower())))
                {
                    //查询到了这个类
                    //开始处理所有方法
                    var SoucreMethods = Items.GetMethods().Where(x => x.GetCustomAttribute(typeof(SocketMethodAttribute)) != null).ToList();
                    foreach (var Item in SoucreMethods)
                    {
                        SocketMethodAttribute SocketMethod = (Item.GetCustomAttribute(typeof(SocketMethodAttribute)) as SocketMethodAttribute);
                        //找到对应方法
                        if (SocketMethod.MethodName.IsNullOrEmpty() ? Routes[2] == Item.Name.ToLower() : SocketMethod.MethodName.ToLower() == Routes[2])
                        {
                            //路由数量大于等于4说明有版本号
                            if (Routes.Count >= 4 && !SocketMethod.MethodVersion.IsNullOrEmpty() && SocketMethod.MethodVersion.ToLower() == Routes.LastOrDefault())
                            {
                                //1.如果启用了Session需要用户实现ISocketSessionHandler处理
                                //2.Invoke方法
                                if (!ExecuteCallSessionHandler(Item, Param.Session))
                                    return new SocketResultDefault { SocketJsonData = ExecuteNoAuthor().ToJson() };
                                return ExecuteCallDataHandler(Items, Item, Param.MiddleResult);
                            }
                            else
                            {
                                //1.如果启用了Session需要用户实现ISocketSessionHandler处理
                                //2.Invoke方法
                                if (!ExecuteCallSessionHandler(Item, Param.Session))
                                    return new SocketResultDefault { SocketJsonData = ExecuteNoAuthor().ToJson() };
                                else
                                    return ExecuteCallDataHandler(Items, Item, Param.MiddleResult);
                            }
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 处理Session
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="Session"></param>
        protected virtual bool ExecuteCallSessionHandler(MethodInfo Method, ISocketSession Session)
        {
            SocketAuthorAttribute MethodAuthor = (Method.GetCustomAttribute(typeof(SocketAuthorAttribute)) as SocketAuthorAttribute);
            if (MethodAuthor != null)
            {
                if (MethodAuthor.UseAuthor)
                {
                    if (DependencyLibrary.SessionDependency.Count() == 0) return false;
                    var SocketSessionHandler = (ISocketSessionHandler)Activator.CreateInstance(DependencyLibrary.SessionDependency.FirstOrDefault());
                    return SocketSessionHandler.Executing(Session);
                }
            }
            return true;
        }
        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="Controller"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
        protected virtual ISocketResult ExecuteCallDataHandler(Type Controller, MethodInfo Method, ISocketResult Param)
        {
            var Ctrl = Activator.CreateInstance(Controller);
            var ParamInfo = Method.GetParameters().FirstOrDefault();
            Object Result;
            if (ParamInfo?.ParameterType == typeof(PageQuery))
            {

                PageQuery TargetParamerter = Param.SocketJsonData.ToModel<PageQuery>();
                Result = ((Task<ActionResult<Object>>)Method.Invoke(Ctrl, new[] { TargetParamerter })).Result.Value;
                if (Result != null) return new SocketResultDefault { SocketJsonData = Result.ToJson() };
            }
            else if (ParamInfo?.ParameterType == typeof(ResultProvider))
            {
                ResultProvider TargetParamerter = ResultProvider.SetValue(null, Param.SocketJsonData.ToModel<Dictionary<string, Object>>());
                Result = ((Task<ActionResult<Object>>)Method.Invoke(Ctrl, new[] { TargetParamerter })).Result.Value;
                if (Result != null) return new SocketResultDefault { SocketJsonData = Result.ToJson() };
            }
            else
            {
                Result = ((Task<ActionResult<Object>>)Method.Invoke(Ctrl, null)).Result.Value;
                if (Result != null) return new SocketResultDefault { SocketJsonData = Result.ToJson() };
            }
            return ExecuteNoAuthor();
        }

        /// <summary>
        /// 返回无权限
        /// </summary>
        /// <returns></returns>
        protected virtual ISocketResult ExecuteNoAuthor()
        {
            return new SocketResultDefault { SocketJsonData = (new { Author = "401 NoAuthor" }).ToJson() };
        }
    }
}

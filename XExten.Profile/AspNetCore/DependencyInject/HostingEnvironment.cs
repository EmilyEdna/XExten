using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.XPlus;
using System.Linq;
using XExten.Profile.Abstractions.Common;
using Microsoft.AspNetCore.Builder;
using XExten.Profile.Core.Common;
using XExten.XCore;

namespace XExten.Profile.AspNetCore.DependencyInject
{
    public static class HostingEnvironment
    {
        /// <summary>
        /// 注册XExten服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="AssemblyName"></param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection RegistXExtenService(this IServiceCollection services, string AssemblyName = "XExten.Profile")
        {
            var Ass = XPlusEx.XAssembly(AssemblyName);
            List<Type> DependencyAssembly = Ass.SelectMany(opt => opt.ExportedTypes.Where(t => t.GetInterfaces().Contains(typeof(IDependency)))).ToList();
            List<Type> SingletonDependencyAssembly = Ass.SelectMany(opt => opt.ExportedTypes.Where(t => t.GetInterfaces().Contains(typeof(ISingletonDependency)))).ToList();
            foreach (Type Item in DependencyAssembly)
            {
                if (Item.IsClass)
                {
                    var TragetService = Item.GetInterfaces().Where(opt => opt != typeof(IDependency)).FirstOrDefault();
                    services.AddSingleton(TragetService, Item);
                }
            }
            foreach (Type Item in SingletonDependencyAssembly)
            {
                if (Item.IsClass)
                    services.AddSingleton(Item);
            }
            return services;
        }
        /// <summary>
        /// 配套使用追踪可视化界面
        /// </summary>
        /// <param name="application"></param>
        /// <param name="UIHost">可视化界面地址 exp:127.0.0.1:9374</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseTraceUI(this IApplicationBuilder application, string UIHost = null)
        {
            if (UIHost.IsNullOrEmpty())
                TracingUIExtension.UIHost = "127.0.0.1:9374";
            else
                TracingUIExtension.UIHost = UIHost;
            return application;
        }
    }
}

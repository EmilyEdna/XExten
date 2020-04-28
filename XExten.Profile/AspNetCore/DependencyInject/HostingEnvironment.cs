using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.XPlus;
using System.Linq;
using XExten.Profile.Abstractions.Common;

namespace XExten.Profile.AspNetCore.DependencyInject
{
    public static class HostingEnvironment
    {
        /// <summary>
        /// 注册XExten服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegistXExtenService(this IServiceCollection services)
        {
            var Ass = XPlusEx.XAssembly();
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
    }
}

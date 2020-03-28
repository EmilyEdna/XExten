using Microsoft.Extensions.DependencyModel;
using XExten.SocketProxy.SocketInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace XExten.SocketProxy.SocketDependency
{
    public class DependencyLibrary
    {
        /// <summary>
        /// 所有程序集
        /// </summary>
        public static IList<Assembly> Assembly { get; set; } = GetAssembly();
        /// <summary>
        /// 获取SocketApi
        /// </summary>
        public static IEnumerable<Type> Dependency => Assembly.SelectMany(item => item.ExportedTypes.Where(t => t.GetInterfaces().Contains(typeof(ISocketDependency))));
        /// <summary>
        /// 自定义Session处理
        /// </summary>
        public static IEnumerable<Type> SessionDependency=> Assembly.SelectMany(item => item.ExportedTypes.Where(t => t.GetInterfaces().Contains(typeof(ISocketSessionHandler))));
        private static IList<Assembly> GetAssembly()
        {
            IList<Assembly> ass = new List<Assembly>();
            var lib = DependencyContext.Default;
            var libs = lib.CompileLibraries.Where(t => !t.Serviceable).Where(t => t.Type == "project").ToList();
            foreach (var item in libs)
            {
                Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(item.Name));
                ass.Add(assembly);
            }
            return ass;
        }
    }
}

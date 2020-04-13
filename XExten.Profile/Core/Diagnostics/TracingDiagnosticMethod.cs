using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using XExten.Profile.Abstractions;
using System.Linq;
using XExten.Profile.Attributes;
using XExten.Profile.AbstractionsDefault;

namespace XExten.Profile.Core.Diagnostics
{
    /// <summary>
    /// 追踪调用方法
    /// </summary>
    public class TracingDiagnosticMethod
    {
        private readonly ITracingDiagnosticProcessor _TracingDiagnosticProcessor;
        private readonly string _DiagnosticName;
        private readonly IParameterResolver[] _RarameterResolvers;
        private readonly MethodReflector _Reflector;
        public TracingDiagnosticMethod(ITracingDiagnosticProcessor TracingDiagnosticProcessor, MethodInfo MethodInfo, String DiagnosticName)
        {
            _TracingDiagnosticProcessor = TracingDiagnosticProcessor;
            _Reflector = MethodInfo.GetReflector();
            _DiagnosticName = DiagnosticName;
            _RarameterResolvers = GetParameterResolvers(MethodInfo).ToArray();
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="DiagnosticName"></param>
        public void Invoke(String DiagnosticName, Object Value)
        {
            if (_DiagnosticName != DiagnosticName) return;
            var args = new Object[_RarameterResolvers.Length];
            for (int i = 0; i < args.Length; i++)
            {
                //设置参数
                args[i] = _RarameterResolvers[i].Resolve(Value);
            }
            //执行追踪的实现类
            _Reflector.Invoke(_TracingDiagnosticProcessor, args);
        }

        /// <summary>
        /// 获取每一个方法的参数
        /// </summary>
        /// <param name="MethodInfo"></param>
        /// <returns></returns>
        private static IEnumerable<IParameterResolver> GetParameterResolvers(MethodInfo MethodInfo)
        {
            foreach (var Item in MethodInfo.GetParameters())
            {
                var Builder = Item.GetCustomAttribute<ParameterResolverAttribute>();
                if (Builder != null)
                {
                    if (Builder is ObjectAttribute ObjectBinder)
                        if (ObjectBinder.TargetType != null)
                            ObjectBinder.TargetType = Item.ParameterType;
                    if (Builder is PropertyAttribute PropertyBinder)
                        if (PropertyBinder.Name == null)
                            PropertyBinder.Name = Item.Name;
                    yield return Builder;
                }
                else
                    yield return new NullParameterResolver();
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using XExten.Profile.AspNetCore.Source;
using System.Linq;
using XExten.XPlus;
using System.Threading.Tasks;
using XExten.XCore;

namespace XExten.Profile.AspNetCore.InvokeTracing
{
    /// <summary>
    /// 方法跟踪
    /// </summary>
    public static class MethodInvoked
    {
        /// <summary>
        /// Invoke method with diagnostic and tracing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Class"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns>方法的结果对象</returns>
        public static Object? ByTraceInvoke<T>(this T Class, string methodName, Object?[]? parameters)
        {
            if (methodName.IsNullOrEmpty()) return null;
            var obj = Class.GetType();
            var method = obj.GetMethod(methodName);
            if (method == null) return null;

            Dictionary<string, object> keyValues = new Dictionary<string, object>();

            List<string> ParameterNames = method.GetParameters().Select(t => t.Name).ToList();

            if (ParameterNames.Count != parameters?.Length) throw new TargetParameterCountException("The number of parameters does not match");

            keyValues.Add("MethodName", method.Name);

            for (int index = 0; index < ParameterNames.Count; index++)
            {
                keyValues.Add(ParameterNames[index], parameters[index].ToJson());
            }
            MethodHandlerDiagnosticListener.MethodListener.ExecuteCommandMethodStar(keyValues);
            return XPlusEx.XTry<Object>(() =>
             {
                 if (!method.ReturnType.Name.Contains("Task"))
                 {
                     var result = method.Invoke(Class, parameters);
                     MethodHandlerDiagnosticListener.MethodListener.ExecuteCommandMethodEnd(result);
                     return result;
                 }
                 else
                 {
                     var result = (dynamic)method.Invoke(Class, parameters);
                     if (((TaskStatus)result.Status) == TaskStatus.Faulted)
                         throw (AggregateException)result.Exception;
                     MethodHandlerDiagnosticListener.MethodListener.ExecuteCommandMethodEnd(result.Result);
                     return result;
                 }
             }, ex =>
             {
                 MethodHandlerDiagnosticListener.MethodListener.ExecuteCommandMethodException(ex.InnerException);
                 return null;
             });
        }

        /// <summary>
        /// Invoke method with diagnostic and tracing
        /// </summary>
        /// <param name="method"></param>
        /// <param name="obj">实例</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Object? ByTraceInvoke(this MethodInfo method, Object? obj, Object?[]? parameters)
        {
            Dictionary<string, object> keyValues = new Dictionary<string, object>();

            List<string> ParameterNames = method.GetParameters().Select(t => t.Name).ToList();

            if (ParameterNames.Count != parameters?.Length) throw new TargetParameterCountException("The number of parameters does not match");

            keyValues.Add("MethodName", method.Name);

            for (int index = 0; index < ParameterNames.Count; index++)
            {
                keyValues.Add(ParameterNames[index], parameters[index].ToJson());
            }
            MethodHandlerDiagnosticListener.MethodListener.ExecuteCommandMethodStar(keyValues);
            return XPlusEx.XTry<Object>(() =>
            {
                if (!method.ReturnType.Name.Contains("Task"))
                {
                    var result = method.Invoke(obj, parameters);
                    MethodHandlerDiagnosticListener.MethodListener.ExecuteCommandMethodEnd(result);
                    return result;
                }
                else
                {
                    var result = (dynamic)method.Invoke(obj, parameters);
                    if (((TaskStatus)result.Status) == TaskStatus.Faulted)
                        throw (AggregateException)result.Exception;
                    MethodHandlerDiagnosticListener.MethodListener.ExecuteCommandMethodEnd(result.Result);
                    return result;
                }
            }, ex =>
            {
                MethodHandlerDiagnosticListener.MethodListener.ExecuteCommandMethodException(ex.InnerException);
                return null;
            });
        }

    }
}

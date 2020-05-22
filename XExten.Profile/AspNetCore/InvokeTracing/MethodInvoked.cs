using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using XExten.Profile.AspNetCore.Source;
using System.Linq;
using XExten.XPlus;
using System.Threading.Tasks;

namespace XExten.Profile.AspNetCore.InvokeTracing
{
    public static class MethodInvoked
    {
        /// <summary>
        /// Invoke method with diagnostic and tracing
        /// </summary>
        /// <param name="method"></param>
        /// <param name="obj"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Object? ByTraceInvoke(this MethodInfo? method, Object? obj, Object?[]? parameters)
        {
            if (method == null) return null;
            if (obj == null) return null;

            Dictionary<string, object> keyValues = new Dictionary<string, object>();

            List<string> ParameterNames = method.GetParameters().Select(t => t.Name).ToList();

            if (ParameterNames.Count != parameters?.Length) throw new TargetParameterCountException("The number of parameters does not match");

            for (int index = 0; index < ParameterNames.Count; index++)
            {
                keyValues.Add(ParameterNames[index], parameters[index]);
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

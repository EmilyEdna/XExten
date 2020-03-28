using BeetleX.Buffers;
using BeetleX.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace XExten.SocketProxy.SocketConfig
{
    public class TypeHandler : IMessageTypeHeader
    {
        private ConcurrentDictionary<Type, string> TypeNames = new ConcurrentDictionary<Type, string>();

        private ConcurrentDictionary<string, Type> NameTypes = new ConcurrentDictionary<string, Type>();

        private Type GetType(string typeName)
        {
            if (!NameTypes.TryGetValue(typeName, out Type result))
            {
                if (typeName == null)
                    throw new Exception($"{typeName} type not found!");
                result = Type.GetType(typeName);
                NameTypes[typeName] = result ?? throw new Exception($"{typeName} type not found!");
            }
            return result;
        }
        private string GetTypeName(Type type)
        {
            if (!TypeNames.TryGetValue(type, out string result))
            {
                TypeInfo info = type.GetTypeInfo();
                if (info.FullName.IndexOf("System") >= 0)
                    result = info.FullName;
                else
                    result = string.Format("{0},{1}", info.FullName, info.Assembly.GetName().Name);
                TypeNames[type] = result;
            }
            return result;
        }

        /// <summary>
        /// 读取类型
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Type ReadType(PipeStream stream)
        {
            string typeName = stream.ReadShortUTF();
            return GetType(typeName);
        }

        public void Register(params Assembly[] assemblies)
        {

        }
        /// <summary>
        /// 写入类型
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stram"></param>
        public void WriteType(object data, PipeStream stram)
        {
            string name = GetTypeName(data.GetType());
            stram.WriteShortUTF(name);
        }
    }
}

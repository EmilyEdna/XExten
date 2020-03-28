using XExten.SocketProxy.SocketConfig;
using XExten.SocketProxy.SocketInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using XExten.SocketProxy.SocketInterface.DefaultImpl;
using XExten.XCore;
using Newtonsoft.Json.Linq;
using XExten.SocketProxy.SocketEnum;
using XExten.SocketProxy.SocketEvent;

namespace XExten.SocketProxy.SocketDependency
{
    public class DependencyCondition
    {
        public static DependencyCondition Instance => new DependencyCondition();

        /// <summary>
        /// 转化结果
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public SocketMiddleData ExecuteMapper(object Param)
        {
            JObject Obj = (Param as string).ToModel<JObject>();
            SendTypeEnum SendType = Enum.Parse<SendTypeEnum>(Obj["SendType"].ToString());
            int? SendPort = Convert.ToInt32(Obj["SendPort"].ToString());
            ISocketResult Result = Obj["MiddleResult"].ToJson().ToModel<SocketResultDefault>();
            ISocketSession Session = Obj["Session"].ToJson().ToModel<SocketSessionDefault>();
            return SocketMiddleData.Middle(SendType, Result, Session, SendPort);
        }
        /// <summary>
        /// 是否回调
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public SendTypeEnum ExecuteIsCall(object Param)
        {
            JObject Obj = (Param as string).ToModel<JObject>();
            return Enum.Parse<SendTypeEnum>(Obj["SendType"].ToString());
        }
        /// <summary>
        /// 映射值
        /// </summary>
        /// <param name="Param"></param>
        public void ExecuteCallData(object Param)
        {
            JObject Obj = (Param as string).ToModel<JObject>();
            var CallData = Obj["MiddleResult"].ToJson().ToModel<SocketResultDefault>();
            CallEvent.Instance().Response = new Dictionary<string, object> { { "SocketJsonData", CallData.SocketJsonData.ToModel<object>() } };
        }
    }
}

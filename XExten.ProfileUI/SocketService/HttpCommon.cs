using BeetleX.FastHttpApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using XExten.ProfileUI.ConfigHelp;

namespace XExten.ProfileUI.SocketService
{
    public class HttpCommon
    {
        public static void InitHttp()
        {
            HttpApiServer http = new HttpApiServer();
            http.Register(typeof(HttpCommon).Assembly);
            http.Options.Port = Convert.ToInt32(ConfigReader.GetSecetion("HttpPort"));
            http.Options.CrossDomain = new OptionsAttribute { AllowOrigin = "*", AllowMethods = "*" };
            http.Open();

        }
    }
}

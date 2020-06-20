using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using XExten.SocketProxy.SocketRoute;
using XExten.XCore;
using XExten.XPlus;
using System.IO;
using XExten.SocketProxy.SocketEnum;
using XExten.SocketProxy.SocketConfig;
using XExten.SocketProxy.SocketInterface.DefaultImpl;


namespace XExten.SocketProxy.SocketDependency
{
    public class DependencyExecute
    {
        public static DependencyExecute Instance => new DependencyExecute();

        private Dictionary<string, List<string>> SocketJson = new Dictionary<string, List<string>>();

        /// <summary>
        /// 查询需要生成APIJson的接口
        /// </summary>
        public SocketMiddleData FindLibrary()
        {
            List<Type> SourceType = DependencyLibrary.Dependency.Where(item => item.GetCustomAttribute(typeof(SocketRouteAttribute)) != null).ToList();
            foreach (var Items in SourceType)
            {
                List<string> Route = new List<string>();
                SocketRouteAttribute SocketRoute = (Items.GetCustomAttribute(typeof(SocketRouteAttribute)) as SocketRouteAttribute);
                string ControllerName = string.Empty;
                if (SocketRoute.ControllerName.IsNullOrEmpty())
                    ControllerName = Items.Name;
                else
                    ControllerName = SocketRoute.ControllerName;
                Items.GetMethods().Where(x => x.GetCustomAttribute(typeof(SocketMethodAttribute)) != null).ToList().ForEach(Item =>
                {
                    SocketMethodAttribute SocketMethod = (Item.GetCustomAttribute(typeof(SocketMethodAttribute)) as SocketMethodAttribute);
                    string SocketUrl = string.Empty;
                    if (SocketMethod.MethodName.IsNullOrEmpty())
                        SocketUrl = $"{SocketRoute.InternalServer}/{ControllerName}/{Item.Name}";
                    else
                        SocketUrl = $"{SocketRoute.InternalServer}/{ControllerName}/{SocketMethod.MethodName}";
                    if (!SocketMethod.MethodVersion.IsNullOrEmpty())
                        SocketUrl = SocketUrl + "/" + SocketMethod.MethodVersion;
                    Route.Add(SocketUrl.ToLower());
                });
                XPlusEx.XTry(() =>
                {
                    var Key = SocketRoute.InternalServer.ToLower();
                    if (SocketJson.ContainsKey(Key))
                        SocketJson[Key].AddRange(Route);
                    else
                        SocketJson.Add(Key, Route);
                }, Ex => throw Ex);
            }
            CreateSocketApiJsonScript();
            return SocketMiddleData.Middle(SendTypeEnum.Init, SocketResultDefault.SetValue(null, SocketJson.ToJson()));
        }
        /// <summary>
        /// 创建API文件
        /// </summary>
        private void CreateSocketApiJsonScript()
        {
            if (SocketJson.Count > 0)
            {
                string Paths = Path.Combine(AppContext.BaseDirectory, @"SocketJsonApi\");
                if (!Directory.Exists(Paths))
                    Directory.CreateDirectory(Paths);
                string FilePath = Path.Combine(Paths, "SocketApi.json");
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    CreateSocketFileContent(FilePath);
                }
                else CreateSocketFileContent(FilePath);
            }
        }
        /// <summary>
        /// 添加文件内容
        /// </summary>
        /// <param name="FilePath"></param>
        private  void CreateSocketFileContent(string FilePath)
        {
            using FileStream Fs = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite);
            using StreamWriter Sw = new StreamWriter(Fs);
            Sw.Write(SocketJson.ToJson());
            Sw.Flush();
            Sw.Close();
            Fs.Close();
        }
    }
}

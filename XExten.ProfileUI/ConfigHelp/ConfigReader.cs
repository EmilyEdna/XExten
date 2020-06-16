using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace XExten.ProfileUI.ConfigHelp
{
    public class ConfigReader
    {
        public static string GetSecetion(string Key)
        {
            ExeConfigurationFileMap FileMap = new ExeConfigurationFileMap();
            FileMap.ExeConfigFilename = Environment.CurrentDirectory + "\\UI.config";
            Configuration Config = ConfigurationManager.OpenMappedExeConfiguration(FileMap, ConfigurationUserLevel.None);
            return Config.AppSettings.Settings[Key].Value;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.XPlus
{
    public class XPlusEx
    {
        #region Func
        #endregion

        #region Sync
        /// <summary>
        /// return a Random Tel
        /// </summary>
        /// <returns></returns>
        public static String XTel()
        {
            String[] PhonesHost = "134,135,136,137,138,139,150,151,152,157,158,159,130,131,132,155,156,133,153,180,181,182,183,185,186,176,187,188,189,177,178".Split(',');
            Random random = new Random();
            int index = random.Next(0, PhonesHost.Length - 1);
            return PhonesHost[index] + (random.Next(100, 888) + 10000).ToString().Substring(1) + (random.Next(1, 9100) + 10000).ToString().Substring(1);
        }
        #endregion

        #region ASync
        /// <summary>
        /// return a Random Tel
        /// </summary>
        /// <returns></returns>
        public static async Task<String> XTelAsync()
        {
            return await Task.Run(() => XTel());
        }
        #endregion
    }
}

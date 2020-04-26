using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Common.CommonAbstraction
{
    /// <summary>
    /// GPS Ponit 轨迹点
    /// </summary>
    public interface IPoint
    {
        /// <summary>
        /// 维度-X
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 经度-Y
        /// </summary>
        public double Longtitude { get; set; }
    }
}

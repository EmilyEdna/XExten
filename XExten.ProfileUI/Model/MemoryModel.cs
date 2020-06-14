using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.ProfileUI.Model
{
    public class MemoryModel
    {
        /// <summary>
        /// 总内存
        /// </summary>
        public string TotalMemory { get; set; }
        /// <summary>
        /// 可用内存
        /// </summary>
        public string AvailMemory { get; set; }
        /// <summary>
        /// 已用内存
        /// </summary>
        public string UsedMemory { get; set; }
    }
}

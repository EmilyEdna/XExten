using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using XExten.ProfileUI.ViewModel;

namespace XExten.ProfileUI.APM
{
    public class Machine
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(ref MemoryInfo info);

        /// <summary>
        /// 格式化容量大小
        /// </summary>
        /// <param name="size">容量（B）</param>
        /// <returns>已格式化的容量</returns>
        public static string FormatSize(double size)
        {
            double d = (double)size;
            int i = 0;
            while ((d > 1024) && (i < 5))
            {
                d /= 1024;
                i++;
            }
            string[] unit = { "B", "KB", "MB", "GB", "TB" };
            return (string.Format("{0} {1}", Math.Round(d, 2), unit[i]));
        }

        /// <summary>
        /// 获得当前内存使用情况
        /// </summary>
        /// <returns></returns>
        public static MemoryInfo GetMemoryStatus()
        {
            MemoryInfo info = new MemoryInfo();
            info.DwLength = (uint)Marshal.SizeOf(info);
            GlobalMemoryStatusEx(ref info);
            return info;
        }

        /// <summary>
        /// 获得当前可用物理内存大小
        /// </summary>
        /// <returns>当前可用物理内存（B）</returns>
        public static ulong GetAvailPhys()
        {
            MemoryInfo info = GetMemoryStatus();
            return info.AllAvailPhys;
        }

        /// <summary>
        /// 获得当前已使用的内存大小
        /// </summary>
        /// <returns>已使用的内存大小（B）</returns>
        public static ulong GetUsedPhys()
        {
            MemoryInfo info = GetMemoryStatus();
            return (info.AllTotalPhys - info.AllAvailPhys);
        }

        /// <summary>
        /// 获得当前总计物理内存大小
        /// </summary>
        /// <returns&amp;gt;总计物理内存大小（B）&amp;lt;/returns&amp;gt;
        public static ulong GetTotalPhys()
        {
            MemoryInfo info = GetMemoryStatus();
            return info.AllTotalPhys;
        }
    }
}

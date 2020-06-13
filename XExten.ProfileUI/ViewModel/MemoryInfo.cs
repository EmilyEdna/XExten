using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace XExten.ProfileUI.ViewModel
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryInfo
    {
        public uint DwLength; //当前结构体大小
        public uint DwMemoryLoad; //当前内存使用率
        public ulong AllTotalPhys; //总计物理内存大小
        public ulong AllAvailPhys; //可用物理内存大小
        public ulong AllTotalPageFile; //总计交换文件大小
        public ulong AllAvailPageFile; //总计交换文件大小
        public ulong AllTotalVirtual; //总计虚拟内存大小
        public ulong AllAvailVirtual; //可用虚拟内存大小
        public ulong AllAvailExtendedVirtual; //保留 这个值始终为0
    }
}

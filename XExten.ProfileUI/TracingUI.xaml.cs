using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XExten.ProfileUI.APM;
using XExten.ProfileUI.ViewModel;

namespace XExten.ProfileUI
{
    /// <summary>
    /// TracingUI.xaml 的交互逻辑
    /// </summary>
    public partial class TracingUI : Window
    {
        public TracingUI()
        {
            InitializeComponent();
            BindMemory();
        }

        public void BindMemory()
        {
            A1.Text = "总内存：" + Machine.FormatSize(Machine.GetTotalPhys());
            A2.Text = "可用内存：" + Machine.FormatSize(Machine.GetAvailPhys());
            A3.Text = "已用内存：" + Machine.FormatSize(Machine.GetUsedPhys());
        }
    }
}

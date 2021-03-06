﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XExten.ProfileUI.SocketService;
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
            SocketCommon.InitSocket();
            HttpCommon.InitHttp();
            DataContext = new MemoryViewModel();
        }
    }
}

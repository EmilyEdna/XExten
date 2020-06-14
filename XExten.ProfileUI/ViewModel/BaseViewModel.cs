using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XExten.ProfileUI.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}

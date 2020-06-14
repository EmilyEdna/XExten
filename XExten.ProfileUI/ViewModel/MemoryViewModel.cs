using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using XExten.ProfileUI.APM;
using XExten.ProfileUI.Model;

namespace XExten.ProfileUI.ViewModel
{
    public class MemoryViewModel : BaseViewModel
    {
        public MemoryViewModel()
        {
            Item = new ObservableCollection<MemoryModel>(new List<MemoryModel> {
                new MemoryModel{
                     TotalMemory =  MachineMemory.FormatSize(MachineMemory.GetTotalPhys()),
                     AvailMemory =  MachineMemory.FormatSize(MachineMemory.GetAvailPhys()),
                     UsedMemory = MachineMemory.FormatSize(MachineMemory.GetUsedPhys())
            }});
        }
        public ObservableCollection<MemoryModel> Item { get; set; }
    }
}

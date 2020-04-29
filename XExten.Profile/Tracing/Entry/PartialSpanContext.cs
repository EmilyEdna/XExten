using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Tracing.Entry
{
    public class PartialSpanContext
    {
        public ChannelLayerType LayerType { get; set; }
        public string URL { get; set; }
        public string Method { get; set; }
        public string Component { get; set; }
        public string Router { get; set; }
        public string Path { get; set; }
    }
}

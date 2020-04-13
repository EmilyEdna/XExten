using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Attributes
{
    public class DiagnosticName : Attribute
    {
        public string Name { get; }
        public DiagnosticName(string name)
        {
            Name = name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Attributes
{
    public class DiagnosticNameAttribute : Attribute
    {
        public string Name { get; }
        public DiagnosticNameAttribute(string name)
        {
            Name = name;
        }
    }
}

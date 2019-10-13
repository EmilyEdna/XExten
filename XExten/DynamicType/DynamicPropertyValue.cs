using System;

namespace XExten.DynamicType
{
    public class DynamicPropertyValue
    {
        private object value;
        private string name;
        private Type type;

        public DynamicPropertyValue(string name, Type type, object value)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");
            if (value == null) throw new ArgumentNullException("value");
            this.name = name;
            this.type = type;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
        }

        public Type Type
        {
            get { return type; }
        }

        public object Value
        {
            get { return value; }
        }
    }
}
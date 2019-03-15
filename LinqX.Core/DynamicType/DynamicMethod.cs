using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace LinqX.Core.DynamicType
{
    public class DynamicMethod
    {
        string name;
        IEnumerable<Type> parameters;
        Type returnType;
        Action<TypeBuilder> buildAction = null;

        public DynamicMethod(string name, IEnumerable<Type> parameters, Type returnType)
        {
            if (name == null) throw new ArgumentNullException("name");

            this.name = name;
            this.parameters = parameters;
            this.returnType = returnType;
        }

        
        public DynamicMethod(string name, IEnumerable<Type> parameters, Type returnType, Action<TypeBuilder> buildAction)
        {
            if (name == null) throw new ArgumentNullException("name");

            this.name = name;
            this.parameters = parameters;
            this.returnType = returnType;
            this.buildAction = buildAction;
        }

        
        public string Name
        {
            get { return name; }
        }

        
        public IEnumerable<Type> Parameters
        {
            get { return parameters; }
        }

        
        public Type ReturnType
        {
            get { return returnType; }
        }

        
        public Action<TypeBuilder> BuildAction
        {
            get { return buildAction; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqX.Core.DynamicType
{
    internal class SignatureBuilder : IEquatable<SignatureBuilder>
    {
        public DynamicMethod[] methods;
        public DynamicProperty[] properties;
        public int hashCode;

        public SignatureBuilder(IEnumerable<DynamicProperty> properties, IEnumerable<DynamicMethod> methods)
        {
            this.properties = properties.ToArray();

            if (methods != null)
                this.methods = methods.ToArray();

            hashCode = 0;
            foreach (DynamicProperty p in properties)
            {
                hashCode ^= p.Name.GetHashCode() ^ p.Type.GetHashCode();
            }
        }

     
        public override int GetHashCode()
        {
            return hashCode;
        }

        
        public override bool Equals(object obj)
        {
            return obj is SignatureBuilder ? Equals((SignatureBuilder)obj) : false;
        }

      
        public bool Equals(SignatureBuilder other)
        {
            if (properties.Length != other.properties.Length) return false;
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name != other.properties[i].Name ||
                    properties[i].Type != other.properties[i].Type) return false;
            }
            return true;
        }
    }
}

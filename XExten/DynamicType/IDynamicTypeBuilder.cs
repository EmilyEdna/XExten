using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XExten.DynamicType
{
    public interface IDynamicTypeBuilder
    {
        /// <summary>
        /// Create a new instance of the dynamic type.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="properties">The collection of properties to create in the type.</param>
        /// <returns>The new instance of the type.</returns>
        object Create(string typeName, IEnumerable<DynamicProperty> properties);

        /// <summary>
        /// Create a new instance of the dynamic type.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="properties">The collection of properties to create in the type.</param>
        /// <param name="methods">The collection of methods to create in the type.</param>
        /// <returns>The new instance of the type.</returns>
        object Create(string typeName, IEnumerable<DynamicProperty> properties, IEnumerable<DynamicMethod> methods);

        /// <summary>
        /// Create a new instance of the dynamic type.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="properties">The collection of properties to create in the type.</param>
        /// <returns>The new instance of the type.</returns>
        object Create(string typeName, IEnumerable<DynamicPropertyValue> properties);

        /// <summary>
        /// Create a new instance of the dynamic type.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="properties">The collection of properties to create in the type.</param>
        /// <param name="methods">The collection of methods to create in the type.</param>
        /// <returns>The new instance of the type.</returns>
        object Create(string typeName, IEnumerable<DynamicPropertyValue> properties, IEnumerable<DynamicMethod> methods);
    }
}

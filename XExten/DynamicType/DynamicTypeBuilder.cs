using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace XExten.DynamicType
{
    public sealed class DynamicTypeBuilder : IDynamicTypeBuilder
    {
        private AssemblyName _assemblyName;
        private AssemblyBuilder _asssemblyBuilder;

        private ModuleBuilder _moduleBuilder;
        private Dictionary<SignatureBuilder, Type> _classes;

        private ReaderWriterLock _rwLock;
        private TypeBuilder _typeBuilder;
        private string _typeName;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="moduleName">The name of the assembly module.</param>
        public DynamicTypeBuilder(string moduleName)
        {
            // Make sure the page reference exists.
            if (moduleName == null) throw new ArgumentNullException("moduleName");

            // Create the nw assembly
            _assemblyName = new AssemblyName(moduleName);
#if NETSTANDARD2_1
            _asssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);
#elif NET461
            _asssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);
#endif

            // Create only one module, therefor the
            // modile name is the assembly name.
            _moduleBuilder = _asssemblyBuilder.DefineDynamicModule(_assemblyName.Name);

            // Get the class unique signature.
            _classes = new Dictionary<SignatureBuilder, Type>();
            _rwLock = new ReaderWriterLock();
        }

        /// <summary>
        /// Create a new instance of the dynamic type.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="properties">The collection of properties to create in the type.</param>
        /// <returns>The new instance of the type.</returns>
        public object Create(string typeName, IEnumerable<DynamicProperty> properties)
        {
            // Make sure the page reference exists.
            if (typeName == null) throw new ArgumentNullException("typeName");
            if (properties == null) throw new ArgumentNullException("properties");

            _typeName = typeName;

            // Return the create type.
            return CreateEx(typeName, properties, null);
        }

        /// <summary>
        /// Create a new instance of the dynamic type.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="properties">The collection of properties to create in the type.</param>
        /// <param name="methods">The collection of methods to create in the type.</param>
        /// <returns>The new instance of the type.</returns>
        public object Create(string typeName, IEnumerable<DynamicProperty> properties, IEnumerable<DynamicMethod> methods)
        {
            // Make sure the page reference exists.
            if (typeName == null) throw new ArgumentNullException("typeName");
            if (properties == null) throw new ArgumentNullException("properties");
            if (methods == null) throw new ArgumentNullException("methods");

            _typeName = typeName;

            // Return the create type.
            return CreateEx(typeName, properties, methods);
        }

        /// <summary>
        /// Create a new instance of the dynamic type.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="properties">The collection of properties to create in the type.</param>
        /// <returns>The new instance of the type.</returns>
        public object Create(string typeName, IEnumerable<DynamicPropertyValue> properties)
        {
            // Make sure the page reference exists.
            if (typeName == null) throw new ArgumentNullException("typeName");
            if (properties == null) throw new ArgumentNullException("properties");

            _typeName = typeName;

            // Create the dynamic type collection
            List<DynamicProperty> prop = new List<DynamicProperty>();
            foreach (DynamicPropertyValue item in properties)
                prop.Add(new DynamicProperty(item.Name, item.Type));

            // Return the create type.
            object instance = CreateEx(typeName, prop.ToArray(), null);
            PropertyInfo[] infos = instance.GetType().GetProperties();

            // Assign each type value
            foreach (PropertyInfo info in infos)
                info.SetValue(instance, properties.First(u => u.Name == info.Name).Value, null);

            // Return the instance with values assigned.
            return instance;
        }

        /// <summary>
        /// Create a new instance of the dynamic type.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="properties">The collection of properties to create in the type.</param>
        /// <param name="methods">The collection of methods to create in the type.</param>
        /// <returns>The new instance of the type.</returns>
        public object Create(string typeName, IEnumerable<DynamicPropertyValue> properties, IEnumerable<DynamicMethod> methods)
        {
            // Make sure the page reference exists.
            if (typeName == null) throw new ArgumentNullException("typeName");
            if (properties == null) throw new ArgumentNullException("properties");
            if (methods == null) throw new ArgumentNullException("methods");

            _typeName = typeName;

            // Create the dynamic type collection
            List<DynamicProperty> prop = new List<DynamicProperty>();
            foreach (DynamicPropertyValue item in properties)
                prop.Add(new DynamicProperty(item.Name, item.Type));

            // Return the create type.
            object instance = CreateEx(typeName, prop.ToArray(), methods);
            PropertyInfo[] infos = instance.GetType().GetProperties();

            // Assign each type value
            foreach (PropertyInfo info in infos)
                info.SetValue(instance, properties.First(u => u.Name == info.Name).Value, null);

            // Return the instance with values assigned.
            return instance;
        }

        /// <summary>
        /// Create a new instance of the dynamic type.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="properties">The collection of properties to create in the type.</param>
        /// <param name="methods">The collection of methods to create in the type.</param>
        /// <returns>The new instance of the type.</returns>
        private object CreateEx(string typeName, IEnumerable<DynamicProperty> properties, IEnumerable<DynamicMethod> methods)
        {
            // Create the dynamic class.
            Type type = GetDynamicClass(properties, methods);

            // Return the new instance of the type.
            return Activator.CreateInstance(type);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="methods"></param>
        /// <returns></returns>
        private Type GetDynamicClass(IEnumerable<DynamicProperty> properties, IEnumerable<DynamicMethod> methods)
        {
            _rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                SignatureBuilder signature = new SignatureBuilder(properties, methods);
                Type type;
                if (!_classes.TryGetValue(signature, out type))
                {
                    type = CreateDynamicClass(signature.properties, signature.methods);
                    _classes.Add(signature, type);
                }
                return type;
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="methods"></param>
        /// <returns></returns>
        private Type CreateDynamicClass(DynamicProperty[] properties, DynamicMethod[] methods)
        {
            LockCookie cookie = _rwLock.UpgradeToWriterLock(Timeout.Infinite);
            try
            {
                string typeName = _typeName;

                try
                {
                    _typeBuilder = _moduleBuilder.DefineType(typeName, TypeAttributes.Class |
                        TypeAttributes.Public, typeof(DynamicClass));

                    CreateConstructor(_typeBuilder);
                    FieldInfo[] fields = GenerateProperties(_typeBuilder, properties);
                    GenerateEquals(_typeBuilder, fields);
                    GenerateGetHashCode(_typeBuilder, fields);

                    if (methods != null)
                        GenerateMethods(_typeBuilder, methods);

                    // Create the type, return the type.
#if NETSTANDARD2_1
                    Type result = _typeBuilder.CreateTypeInfo();
#elif NET461
                    Type result = _typeBuilder.CreateType();
#endif
                    return result;
                }
                finally { }
            }
            finally
            {
                _rwLock.DowngradeFromWriterLock(ref cookie);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="typeBuilder"></param>
        private void CreateConstructor(TypeBuilder typeBuilder)
        {
            // Create the default constructor.
            ConstructorInfo baseConstructorInfo = typeof(object).GetConstructor(new Type[0]);
            ConstructorBuilder constructorBuilder =
                typeBuilder.DefineConstructor(
                           MethodAttributes.Public,
                           CallingConventions.Standard,
                           Type.EmptyTypes);

            // Create the base call operations.
            ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, baseConstructorInfo);
            ilGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="methods"></param>
        private void GenerateMethods(TypeBuilder tb, DynamicMethod[] methods)
        {
            for (int i = 0; i < methods.Length; i++)
            {
                DynamicMethod dm = methods[i];
                MethodBuilder mb;
                ILGenerator mdMethod;

                // If a build action exists.
                if (dm.BuildAction != null)
                {
                    // Execute the custom build action.
                    dm.BuildAction(tb);
                }
                else if (dm.ReturnType != typeof(void) && dm.Parameters != null)
                {
                    mb = tb.DefineMethod(dm.Name, MethodAttributes.Public |
                        MethodAttributes.SpecialName | MethodAttributes.HideBySig, dm.ReturnType, dm.Parameters.ToArray());
                    mdMethod = mb.GetILGenerator();

                    // Load the instance it belongs to (argument zero is the instance)
                    LocalBuilder localBuilder = mdMethod.DeclareLocal(dm.ReturnType);
                    mdMethod.Emit(OpCodes.Ldloc, localBuilder);
                    mdMethod.Emit(OpCodes.Ret);
                }
                else if (dm.ReturnType != typeof(void) && dm.Parameters == null)
                {
                    mb = tb.DefineMethod(dm.Name, MethodAttributes.Public |
                        MethodAttributes.SpecialName | MethodAttributes.HideBySig, dm.ReturnType, null);
                    mdMethod = mb.GetILGenerator();

                    // Load the instance it belongs to (argument zero is the instance)
                    LocalBuilder localBuilder = mdMethod.DeclareLocal(dm.ReturnType);
                    mdMethod.Emit(OpCodes.Ldloc, localBuilder);
                    mdMethod.Emit(OpCodes.Ret);
                }
                else if (dm.ReturnType == typeof(void) && dm.Parameters != null)
                {
                    mb = tb.DefineMethod(dm.Name, MethodAttributes.Public |
                        MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(void), dm.Parameters.ToArray());
                    mdMethod = mb.GetILGenerator();

                    // Load the instance it belongs to (argument zero is the instance)
                    mdMethod.Emit(OpCodes.Ret);
                }
                else
                {
                    mb = tb.DefineMethod(dm.Name, MethodAttributes.Public |
                        MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(void), null);
                    mdMethod = mb.GetILGenerator();

                    // Load the instance it belongs to (argument zero is the instance)
                    mdMethod.Emit(OpCodes.Ret);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private FieldInfo[] GenerateProperties(TypeBuilder tb, DynamicProperty[] properties)
        {
            FieldInfo[] fields = new FieldBuilder[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                DynamicProperty dp = properties[i];
                FieldBuilder fb = tb.DefineField("_" + dp.Name, dp.Type, FieldAttributes.Private);
                PropertyBuilder pb = tb.DefineProperty(dp.Name, PropertyAttributes.HasDefault, dp.Type, null);
                MethodBuilder mbGet = tb.DefineMethod("get_" + dp.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    dp.Type, Type.EmptyTypes);
                ILGenerator genGet = mbGet.GetILGenerator();
                genGet.Emit(OpCodes.Ldarg_0);
                genGet.Emit(OpCodes.Ldfld, fb);
                genGet.Emit(OpCodes.Ret);
                MethodBuilder mbSet = tb.DefineMethod("set_" + dp.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    null, new Type[] { dp.Type });
                ILGenerator genSet = mbSet.GetILGenerator();
                genSet.Emit(OpCodes.Ldarg_0);
                genSet.Emit(OpCodes.Ldarg_1);
                genSet.Emit(OpCodes.Stfld, fb);
                genSet.Emit(OpCodes.Ret);
                pb.SetGetMethod(mbGet);
                pb.SetSetMethod(mbSet);
                fields[i] = fb;
            }
            return fields;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="fields"></param>
        private void GenerateEquals(TypeBuilder tb, FieldInfo[] fields)
        {
            MethodBuilder mb = tb.DefineMethod("Equals",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(bool), new Type[] { typeof(object) });
            ILGenerator gen = mb.GetILGenerator();
            LocalBuilder other = gen.DeclareLocal(tb);
            Label next = gen.DefineLabel();
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Isinst, tb);
            gen.Emit(OpCodes.Stloc, other);
            gen.Emit(OpCodes.Ldloc, other);
            gen.Emit(OpCodes.Brtrue_S, next);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ret);
            gen.MarkLabel(next);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                next = gen.DefineLabel();
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ldloc, other);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new Type[] { ft, ft }), null);
                gen.Emit(OpCodes.Brtrue_S, next);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ret);
                gen.MarkLabel(next);
            }
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ret);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="fields"></param>
        private void GenerateGetHashCode(TypeBuilder tb, FieldInfo[] fields)
        {
            MethodBuilder mb = tb.DefineMethod("GetHashCode",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(int), Type.EmptyTypes);
            ILGenerator gen = mb.GetILGenerator();
            gen.Emit(OpCodes.Ldc_I4_0);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new Type[] { ft }), null);
                gen.Emit(OpCodes.Xor);
            }
            gen.Emit(OpCodes.Ret);
        }
    }
}
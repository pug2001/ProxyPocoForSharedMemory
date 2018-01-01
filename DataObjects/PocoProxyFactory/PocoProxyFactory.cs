using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DataObjects.PocoProxyFactory
{
    public static class PocoProxyFactory<TData> where TData : class, new()
    {
        private static Type _pocoType;
        private static FieldBuilder _pocoField;
        public static Type PocoType => _pocoType ?? (_pocoType = typeof(TData));

        public static TData CreateProxyObject(TData poco)
        {
            var myType = CreateProxyType().AsType();

//            var ctor = ((System.RuntimeType)myType).GetConstructor(new Type[] {PocoType});
//            return (TData)ctor.Invoke(new { poco });
            return (TData)Activator.CreateInstance(myType, poco);
        }

        public static TypeInfo CreateProxyType()
        {
            TypeBuilder tb = GetTypeBuilder();
            BuildConstructor(tb);
            BuildProxyProperties(PocoType, tb);


            var objectType = tb.CreateTypeInfo();
            return objectType;
        }

        private static void BuildProxyProperties(Type pocoType, TypeBuilder tb)
        {
            foreach (var pi in pocoType.GetRuntimeProperties())
            {
                if (!pi.PropertyType.IsPointer)
                {
                    MapToBaseProperty(tb, pi.Name, pi.PropertyType);
                }
                else
                {
                    CreateProxyProperty(tb, pi);
                }
            }
        }

        private static void BuildConstructor(TypeBuilder tb)
        {
            BuildConstructorFields(tb);

            Type[] ctorParams = new Type[] {PocoType};

            //ConstructorBuilder constructor =
            //    tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
            //                                MethodAttributes.RTSpecialName);
            ConstructorBuilder constructor =
                tb.DefineConstructor(MethodAttributes.Public
                                            //MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName
                                            , CallingConventions.Standard
                                            ,ctorParams);
            constructor.DefineParameter(1, ParameterAttributes.In, "_poco");
            ILGenerator ctorIl = constructor.GetILGenerator();

            // NOTE: ldarg.0 holds the "this" reference - ldarg.1, ldarg.2, and ldarg.3
            // hold the actual passed parameters. ldarg.0 is used by instance methods
            // to hold a reference to the current calling object instance. Static methods
            // do not use arg.0, since they are not instantiated and hence no reference
            // is needed to distinguish them. 

            ctorIl.Emit(OpCodes.Ldarg_0);

            // Here, we wish to create an instance of System.Object by invoking its
            // constructor, as specified above.

            Type objType = Type.GetType("System.Object");
            var objCtors = objType.GetTypeInfo().DeclaredConstructors;
            ctorIl.Emit(OpCodes.Call, objCtors.First());

            // Now, we'll load the current instance ref in arg 0, along
            // with the value of parameter "x" stored in arg 1, into stfld.

            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Ldarg_1);
            ctorIl.Emit(OpCodes.Stfld, _pocoField);
            // Our work complete, we return.
            ctorIl.Emit(OpCodes.Ret);
        }

        private static void BuildConstructorFields(TypeBuilder tb)
        {
            _pocoField = tb.DefineField("_poco", typeof(TData), FieldAttributes.Private);
        }

        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = $"{typeof(TData)}-{Guid.NewGuid().ToString()}";
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("PocoFactory");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                    PocoType);
            return tb;
        }

        private static void CreateProxyProperty(TypeBuilder tb, PropertyInfo pi)
        {
            return;
        }

        private static void MapToBaseProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            var propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            EmitPropertyGet(tb, propertyName, propertyType, propertyBuilder);
            EmitPropertySet(tb, propertyName, propertyType, propertyBuilder);
        }

        private static void EmitPropertySet(TypeBuilder tb, string propertyName, Type propertyType,
            PropertyBuilder propertyBuilder)
        {
            var tdataPropMethodSet = typeof(TData).GetRuntimeMethods().First(x=>x.Name == "set_" + propertyName && x.GetParameters().Length==1 && x.GetParameters().First().ParameterType == propertyType);// &&, new Type[] {propertyType});

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new[] {propertyType});

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            //Label modifyProperty = setIl.DefineLabel();
            //Label exitSet = setIl.DefineLabel();

            //setIl.MarkLabel(modifyProperty);
            ////            setIl.Emit(OpCodes.Ldarg_0);
            //setIl.Emit(OpCodes.Ldfld, _pocoField);
            //setIl.Emit(OpCodes.Ldarg_1);
            ////setIl.Emit(OpCodes.Callvirt, tdataPropMethodSet);

            //setIl.Emit(OpCodes.Nop);
            //setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        private static void EmitPropertyGet(TypeBuilder tb, string propertyName, Type propertyType, PropertyBuilder propertyBuilder)
        {
            var propMethodName = "get_" + propertyName;
            var getPropMthdBldr = tb.DefineMethod(propMethodName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType,
                Type.EmptyTypes);
            var tdataPropMethodGet = typeof(TData).GetRuntimeMethod(propMethodName, new Type[0]);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, _pocoField);
            getIl.Emit(OpCodes.Callvirt, tdataPropMethodGet);
            getIl.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getPropMthdBldr);
        }
    }
}

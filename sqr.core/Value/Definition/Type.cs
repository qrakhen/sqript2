using Qrakhen.Sqr.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Qrakhen.Sqr.Core 
{ 
    public class Type
    {
        private static readonly Storage<string, Type> definitions = new Storage<string, Type>();
        public static List<string> typeList => definitions.Keys.ToList();

        public static Type Value        => CoreModule.instance.getType("Value");
        public static Type Boolean      => CoreModule.instance.getType("Boolean");
        public static Type Float        => CoreModule.instance.getType("Float");
        public static Type Integer      => CoreModule.instance.getType("Integer");
        public static Type Number       => CoreModule.instance.getType("Number");
        public static Type String       => CoreModule.instance.getType("String");
        public static Type Array        => CoreModule.instance.getType("Array");
        public static Type List         => CoreModule.instance.getType("List");
        public static Type Qollection   => CoreModule.instance.getType("Qollection");
        public static Type Qallable     => CoreModule.instance.getType("Qallable");
        public static Type Objeqt       => CoreModule.instance.getType("Objeqt");
        public static Type Variable     => CoreModule.instance.getType("Variable");
        public static Type Qlass        => CoreModule.instance.getType("Qlass");
        public static Type Module       => CoreModule.instance.getType("Module");

        [NativeField] public readonly string id;
        [NativeField] public readonly string name;
        [NativeField] public readonly NativeType nativeType;
        [NativeField] public readonly Module module;
        [NativeField] public readonly Type extends;
        [NativeField] public readonly System.Type nativeClass;
        [NativeField] public readonly Storage<string, Field> fields;
        [NativeField] public readonly Storage<string, Method> methods;

        public bool isPrimitive => (nativeType & NativeType.Primitive) > nativeType;
        public bool isNative => (nativeType != NativeType.Instance);
                
        private Type(Args args)
        {
            name = args.name;
            module = args.module;
            nativeType = args.nativeType;
            fields = args.fields ?? new Storage<string, Field>();
            methods = args.methods ?? new Storage<string, Method>();
            extends = args.extends;
            nativeClass = args.nativeClass;        

            if (extends != null)
                extend();
        }

        private void extend()
        {
            foreach (var f in extends.fields) {
                if (fields[f.Key] == null)
                    fields[f.Key] = f.Value;
            }

            foreach (var m in extends.methods) {
                if (methods[m.Key] == null)
                    methods[m.Key] = m.Value;
            }
        }

        public Value spawn(Qontext qontext, Value[] parameters)
        {
            if (nativeType == NativeType.Static)
                throw new SqrTypeError("can not instantiate a static qlass!");
            Value obj = null;
            if (nativeClass != null && nativeClass != typeof(Instance)) {
                obj = (Value)Activator.CreateInstance(nativeClass, parameters);
            } else {
                obj = new Instance(qontext, this);
                foreach (var f in fields.Values) {
                    obj.fields[f.name] = (obj as Instance).qontext.register(f.name, f.defaultValue, f.isReference, f.type, f.isReadonly);
                }
                if (methods.contains(name)) {
                    methods[name].makeQallable(obj).execute(parameters, qontext);
                }
            }
            return obj;
        }

        public Value invoke(Method method, Value invoker, Value[] parameters, Qontext qontext)
        {
            return method.funqtion.execute(parameters, qontext, invoker);
        }

        public override string ToString()
        {
            return name;
        }

        public static Type create(System.Type systemType, Args args)
        {
            args.nativeClass = systemType;

            if (args.fields == null)
                args.fields = new Storage<string, Field>();
            foreach (var f in buildNativeFields(systemType))
                args.fields[f.name] = f;

            if (args.methods == null)
                args.methods = new Storage<string, Method>();
            foreach (var m in buildNativeMethods(systemType))
                args.methods[m.name] = m;

            return new Type(args);
		}

		/*public static Type createLibType(System.Type libType, Args args) 
        {
			args.nativeClass = libType;

			if (args.fields == null)
				args.fields = new Storage<string, Field>();
			foreach (var f in buildLibFields(libType))
				args.fields[f.name] = f;

			if (args.methods == null)
				args.methods = new Storage<string, Method>();
			foreach (var m in buildLibMethods(libType))
				args.methods[m.name] = m;

			return new Type(args);
		}*/

		private static List<Field> buildNativeFields(System.Type type) 
        {
			var fields = new List<Type.Field>();
			foreach (var f in type
				.GetFields()
				.Where(_ =>
					Attribute.GetCustomAttribute(_, typeof(NativeFieldAttribute)) != null)) {
				fields.Add(new Type.Field(new IDeclareInfo() {
					name = f.Name,
					type = null,
					access = Access.Public,
					isReadonly = Attribute.GetCustomAttribute(f, typeof(NativeGetterAttribute)) != null
				}));
			}
			return fields;
		}

		private static List<Method> buildNativeMethods(System.Type type) {
			var methods = new List<Method>();
			foreach (var m in type
				.GetMethods()
				.Where(_ =>
					Attribute.GetCustomAttribute(_, typeof(NativeMethodAttribute)) != null)) {
				methods.Add(new Type.Method(
					new InternalFunqtion((v, q, s) => { return (Value) m.Invoke(m.IsStatic ? null : s?.obj, v); }),
					new IDeclareInfo() {
						name = m.Name,
						type = null,
						access = Access.Public
					}));
			}
			return methods;
		}

        public string render()
        {
            var r = "Type <" + name + ">:\n";
            r += "  Fields:\n";
            foreach (var f in fields.Values) {
                r += "   " + f.access.ToString() + " " + f.name + ": " + f.type.ToString() + "\n";
            }
            r += "  Methods:\n";
            foreach (var m in methods.Values) {
                r += "   " + m.access.ToString() + " " + m.name + ": " + m.type?.ToString() + " " + m.funqtion.ToString() + "\n";
            }
            return r;
        }

        public class Field
        {
            public readonly string name;
            public readonly Type type;
            public readonly Access access;
            public readonly bool isReference;
            public readonly bool isReadonly;
            public readonly Value defaultValue = null;

            public Field(IDeclareInfo info = new IDeclareInfo())
            {
                foreach (var f in GetType().GetFields()) {
                    f.SetValue(this, info.GetType().GetField(f.Name).GetValue(info));
                }
            }
        }

        public class Method
        {
            public readonly string name;
            public readonly Type type;
            public readonly Access access;
            public readonly bool isReference;
            public readonly Funqtion funqtion;

            public Method(Funqtion funqtion, IDeclareInfo info = new IDeclareInfo())
            {
                this.funqtion = funqtion;
                foreach (var f in GetType().GetFields()) {
                    if (f.Name == "funqtion") continue;
                    f.SetValue(this, info.GetType().GetField(f.Name).GetValue(info));
                }
            }

            public Value execute(Value[] parameters, Qontext qontext, Value self = null)
                => funqtion.execute(parameters, qontext, self);

            public Qallable makeQallable(Value target)
                => new Qallable(funqtion, target);
        }

        public enum Access
        {
            Public = 1,
            Protected = 2,
            Private = 3
        }

        public struct Args
        {
            public string name;
            public Module module;
            public Type extends;
            public System.Type nativeClass;
            public NativeType nativeType;
            public Storage<string, Field> fields;
            public Storage<string, Method> methods;
        }
    }

    [Flags]
    public enum NativeType
    {
        None = default,
        Boolean = BitFlag._1,
        Byte = BitFlag._2,
        Float = BitFlag._3,
        Integer = BitFlag._4,
        Number = Float | Integer,

        String = BitFlag._5,
        Primitive = Boolean | Number | String,

        Array = BitFlag._6,
        List = BitFlag._7,
        Qollection = Array | List,

        Objeqt = BitFlag._8,
        Instance = BitFlag._9,
        Static = BitFlag._10,
        Variable = BitFlag._11,
        Funqtion = BitFlag._12,
        Null = BitFlag._13,
        Module = BitFlag._14

    }
}

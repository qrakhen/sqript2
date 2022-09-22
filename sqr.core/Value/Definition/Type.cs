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

        public static Type Value        => get("Value");
        public static Type Boolean      => get("Boolean");
        public static Type Float        => get("Float");
        public static Type Integer      => get("Integer");
        public static Type Number       => get("Number");
        public static Type String       => get("String");
        public static Type Array        => get("Array");
        public static Type List         => get("List");
        public static Type Qollection   => get("Qollection");
        public static Type Qallable     => get("Qallable");
        public static Type Objeqt       => get("Objeqt");
        public static Type Variable     => get("Variable");
        public static Type Qlass        => get("Qlass");

        private static readonly Module coreModule = new Module("Core", new Module("Sqript", null));

        [NativeField] public readonly string id;
        [NativeField] public readonly string name;
        [NativeField] public readonly NativeType nativeType;
        [NativeField] public readonly Module module;
        [NativeField] public readonly Type extends;
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

            id = buildTypeId(this);

            if (definitions.contains(name))
                throw new SqrError("can not redeclare type " + name);

            if (definitions[args.name] == null) {
                definitions[name.ToLower()] = this;
            } else {
                definitions[id.ToLower()] = this;
            }

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

        // native types are instantiated by just using new() since theyre hard coded
        public Instance spawn(Qontext qontext, Value[] parameters)
        {
            var obj = new Instance(qontext, this);
            foreach (var f in fields.Values) {
                obj.fields[f.name] = obj.qontext.register(f.name, f.defaultValue, f.isReference, f.type, f.isReadonly);
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

        public static Type get(string name)
        {
            return definitions[name.ToLower()];
        }

        public static Type register(System.Type systemType, Args args)
        {
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

        public static string buildTypeId(Type type)
        {
            var id = type.name;
            var m = type.module;
            while(m != null)  {
                id = m.name + "/" + id;
                m = m.parent;
            }
            return id;
        }

        private static List<Field> buildNativeFields(System.Type type)
        {
            var fields = new List<Type.Field>();
            foreach (var f in type
                .GetFields()
                .Where(_ =>
                    Attribute.GetCustomAttribute(_, typeof(NativeFieldAttribute)) != null)) {
                    fields.Add(new Type.Field(new IDeclareInfo() {
                        name = f.Name,
                        type = get(f.FieldType.Name),
                        access = Access.Public
                    }));
            }
            return fields;
        }

        private static List<Method> buildNativeMethods(System.Type type)
        {
            var methods = new List<Method>();
            foreach (var m in type
                .GetMethods()
                .Where(_ =>
                    Attribute.GetCustomAttribute(_, typeof(NativeMethodAttribute)) != null)) {
                    methods.Add(new Type.Method(
                        new InternalFunqtion((v, q, s) => { return (Value)m.Invoke(s.obj, v); }),
                        new IDeclareInfo() {
                            name = m.Name,
                            type = get(m.ReturnType.Name),
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
                if (type == null)
                    type = Type.Value;
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
            public NativeType nativeType;
            public Storage<string, Field> fields;
            public Storage<string, Method> methods;
        }

        static Type()
        {
            var value = register(typeof(Value), new Args
            {
                name = "Value",
                nativeType = NativeType.None,
                module = coreModule
            });

            var _string = register(typeof(String), new Args
            {
                name = "String",
                nativeType = NativeType.String,
                extends = value,
                module = coreModule
            });

            var number = register(typeof(Number), new Args {
                name = "Number",
                nativeType = NativeType.Number,
                extends = value,
                module = coreModule
            });

            var integer = register(typeof(Integer), new Args {
                name = "Integer",
                nativeType = NativeType.Integer,
                extends = value,
                module = coreModule,
            });

            var boolean = register(typeof(Boolean), new Args {
                name = "Boolean",
                nativeType = NativeType.Boolean,
                extends = value,
                module = coreModule
            });

            var qollection = register(typeof(Qollection), new Args {
                name = "Qollection",
                nativeType = NativeType.Qollection,
                extends = value,
                module = coreModule
            });

            var objeqt = register(typeof(Objeqt), new Args {
                name = "Objeqt",
                nativeType = NativeType.Objeqt,
                extends = value,
                module = coreModule                
            });

            var array = register(typeof(Array), new Args {
                name = "Array",
                nativeType = NativeType.Array,
                extends = value,
                module = coreModule
            });

            var qallable = register(typeof(Qallable), new Args {
                name = "Qallable",
                nativeType = NativeType.Funqtion,
                extends = value,
                module = coreModule,
            });

            var variable = register(typeof(Variable), new Args {
                name = "Variable",
                nativeType = NativeType.Variable,
                extends = value,
                module = coreModule
            });
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
        Variable = BitFlag._10,
        Funqtion = BitFlag._11,
        Null = BitFlag._12,

    }
}

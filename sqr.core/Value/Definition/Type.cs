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

        public Type() { }
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
        public Instance spawn(Value[] parameters)
        {
            return new Instance(this);
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

        public static Type register(Args args)
        {
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

        public class Field
        {
            public readonly string name;
            public readonly Type type;
            public readonly Access access;
            public readonly bool isReference;
            public readonly bool isReadonly;

            public Field(IDeclareInfo info = new IDeclareInfo())
            {
                foreach (var f in GetType().GetFields()) {
                    f.SetValue(this, info.GetType().GetField(f.Name));
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
                    f.SetValue(this, info.GetType().GetField(f.Name));
                }
            }

            public Value execute(Value[] parameters, Qontext qontext, Value self = null)
                => funqtion.execute(parameters, qontext, self);

            public Qallable makeQallable()
                => new Qallable(funqtion);
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
            var value = register(new Args
            {
                name = "Value",
                nativeType = NativeType.None,
                fields = null,
                module = coreModule,
                methods = new Storage<string, Method>() {
                    { "toString", new Method(new InternalFunqtion((p, q, self) => self.toString())) },
                    { "type", new Method(new InternalFunqtion((p, q, self) => new String(self.type.name))) }
                }
            });

            var _string = register(new Args
            {
                name = "String",
                nativeType = NativeType.String,
                fields = null,
                extends = value,
                module = coreModule,
                methods = new Storage<string, Method>() {
                { "span", new Method(
                    new InternalFunqtion((p, q, self) => (self as Core.String).span(p[0], p[1]))) }
                }
            });

            var number = register(new Args {
                name = "Number",
                nativeType = NativeType.Number,
                fields = null,
                extends = value,
                module = coreModule,
                methods = new Storage<string, Method>() {
                { "xxx", new Method(
                    new InternalFunqtion((p, q, self) => Core.Value.Null)) }
                }
            });

            var boolean = register(new Args {
                name = "Boolean",
                nativeType = NativeType.Boolean,
                fields = null,
                extends = value,
                module = coreModule,
                methods = null
            });

            var qollection = register(new Args {
                name = "Qollection",
                nativeType = NativeType.Qollection,
                fields = null,
                extends = value,
                module = coreModule,
                methods = new Storage<string, Method>() {
                { "length", new Method(
                    new InternalFunqtion((p, q, self) => new Number((self.obj as Qollection).length))) }
                }
            });

            var objeqt = register(new Args {
                name = "Objeqt",
                nativeType = NativeType.Objeqt,
                fields = null,
                extends = value,
                module = coreModule,
                methods = new Storage<string, Method>() {
                { "xxx", new Method(
                    new InternalFunqtion((p, q, self) => Core.Value.Null)) }
                }
            });

            var array = register(new Args {
                name = "Array",
                nativeType = NativeType.Array,
                fields = null,
                extends = value,
                module = coreModule,
                methods = new Storage<string, Method>() {
                { "xxx", new Method(
                    new InternalFunqtion((p, q, self) => Core.Value.Null)) }
                }
            });

            var qallable = register(new Args {
                name = "Qallable",
                nativeType = NativeType.Funqtion,
                fields = null,
                extends = value,
                module = coreModule,
                methods = new Storage<string, Method>() {
                { "xxx", new Method(
                    new InternalFunqtion((p, q, self) => Core.Value.Null)) }
                }
            });

            var variable = register(new Args {
                name = "Variable",
                nativeType = NativeType.Variable,
                fields = null,
                extends = value,
                module = coreModule,
                methods = new Storage<string, Method>() {
                { "xxx", new Method(
                    new InternalFunqtion((p, q, self) => Core.Value.Null)) },
                { "type", new Method(
                    new InternalFunqtion((p, q, self) => new String(self.type.name + "<" + self.obj.type.name + ">"))) }
                }
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

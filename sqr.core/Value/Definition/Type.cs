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

        public static Type Value => definitions["Value"];
        public static Type Boolean => definitions["Boolean"];
        public static Type Float => definitions["Float"];
        public static Type Integer => definitions["Integer"];
        public static Type Number => definitions["Number"];
        public static Type String => definitions["String"];
        public static Type Array => definitions["Array"];
        public static Type List => definitions["List"];
        public static Type Qollection => definitions["Qollection"];
        public static Type Qallable => definitions["Qallable"];
        public static Type Objeqt => definitions["Objeqt"];
        public static Type Variable => definitions["Variable"];

        private static readonly Module coreModule = new Module("Core", new Module("Sqript", null));

        public readonly string id;
        public readonly string name;
        public readonly Module module;
        public readonly Type extends;
        public readonly NativeType nativeType;
        public readonly Storage<string, Field> fields;
        public readonly Storage<string, Method> methods;

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

            definitions[name] = this;

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
        public Instance spawn(Funqtion.ProvidedParam[] parameters)
        {
            return new Instance(this);
        }

        public Value invoke(Method method, Value invoker, Funqtion.ProvidedParam[] parameters, Qontext qontext)
        {
            return method.funqtion.execute(parameters, qontext, invoker);
        }

        public override string ToString()
        {
            return id;
        }

        public static Type get(string name)
        {
            return definitions[name];
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
            public readonly Access access;
            public readonly Type type;

            public Field(Access access, Type type)
            {
                this.type = type;
                this.access = access;
            }
        }

        public class Method
        {
            public readonly Access access;
            public readonly Funqtion funqtion;

            public Method(Funqtion funqtion, Access access = Access.Public)
            {
                this.funqtion = funqtion;
                this.access = access;
            }

            public Value makeValue() => new Value<Funqtion>(funqtion, Type.Qallable);
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
                methods = new Storage<string, Method>() {
                { "toString", new Method(
                    new InternalFunqtion((p, self) => self.toString())) }
                }
            });

            var _string = register(new Args
            {
                name = "String",
                nativeType = NativeType.String,
                fields = null,
                extends = value,
                methods = new Storage<string, Method>() {
                { "span", new Method(
                    new InternalFunqtion((p, self) => (self as Core.String).span(p[0].value, p[1].value))) }
                }
            });

            var number = register(new Args {
                name = "Number",
                nativeType = NativeType.Number,
                fields = null,
                extends = value,
                methods = new Storage<string, Method>() {
                { "xxx", new Method(
                    new InternalFunqtion((p, self) => Core.Value.Null)) }
                }
            });

            var boolean = register(new Args {
                name = "Boolean",
                nativeType = NativeType.Boolean,
                fields = null,
                extends = value,
                methods = null
            });

            var qollection = register(new Args {
                name = "Qollection",
                nativeType = NativeType.Qollection,
                fields = null,
                extends = value,
                methods = new Storage<string, Method>() {
                { "length", new Method(
                    new InternalFunqtion((p, self) => new Number((self as Qollection).length))) }
                }
            });

            var objeqt = register(new Args {
                name = "Objeqt",
                nativeType = NativeType.Objeqt,
                fields = null,
                extends = value,
                methods = new Storage<string, Method>() {
                { "xxx", new Method(
                    new InternalFunqtion((p, self) => Core.Value.Null)) }
                }
            });

            var array = register(new Args {
                name = "Array",
                nativeType = NativeType.Array,
                fields = null,
                extends = value,
                methods = new Storage<string, Method>() {
                { "xxx", new Method(
                    new InternalFunqtion((p, self) => Core.Value.Null)) }
                }
            });
        }
    }

    [Flags]
    public enum NativeType
    {
        None = default,
        Boolean = 1,
        Byte = 2,
        Float = 4,
        Integer = 8,
        Number = Float | Integer,

        String = 16,
        Primitive = Boolean | Number | String,

        Array = 32,
        List = 64,
        Qollection = Array | List,

        Objeqt = 128,
        Instance = 256,
        Variable = 512,
        Funqtion = 1024,
        Null = 2048
    }
}

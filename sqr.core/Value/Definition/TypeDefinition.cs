using Qrakhen.Sqr.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qrakhen.Sqr.Core 
{ 
    public class TypeDefinition
    {
        private static readonly Storage<string, TypeDefinition> definitions = new Storage<string, TypeDefinition>();
        public static List<string> typeList => definitions.Keys.ToList();

        public static readonly TypeDefinition
            Value = definitions["Value"],
            Boolean = definitions["Boolean"],
            Number = definitions["Number"],
            String = definitions["String"],
            Qollection = definitions["Qollection"],
            Qallable = definitions["Qallable"],
            Objeqt = definitions["Objeqt"];

        public readonly string name;
        public readonly string module; //wie namespace
        public readonly TypeDefinition extends;
        public readonly NativeType nativeType;
        public readonly Storage<string, Field> fields;
        public readonly Storage<string, Method> methods;

        public bool isPrimitive => (nativeType & NativeType.Primitive) > nativeType;
        public bool isNative => (nativeType != NativeType.Instance);

        private TypeDefinition(Args args)
        {
            name = args.name;
            module = args.module;
            nativeType = args.nativeType;
            fields = args.fields ?? new Storage<string, Field>();
            methods = args.methods ?? new Storage<string, Method>();

            if (definitions.contains(name))
                throw new SqrError("can not redeclare type " + name);

            definitions[name] = this;
        }

        public TypeDefinition() { }

        // natives types are instantiated by just using new() since theyre hard coded
        public Instance spawn(Funqtion.ProvidedParam[] parameters)
        {
            return new Instance(this);
        }

        public Value invoke(Method method, Value invoker, Funqtion.ProvidedParam[] parameters, Qontext qontext)
        {
            return method.funqtion.execute(parameters, qontext, invoker);
        }

        public static TypeDefinition get(string name)
        {
            return definitions[name];
        }

        public static TypeDefinition register(Args args)
        {
            return new TypeDefinition(args);
        }

        public class Field
        {
            public readonly Access access;
            public readonly string type;

            public Field(Access access, string type)
            {
                this.type = type;
                this.access = access;
            }
        }

        public class Method
        {
            public readonly Access access;
            public readonly Funqtion funqtion;

            public Method(Access access, Funqtion funqtion)
            {
                this.access = access;
                this.funqtion = funqtion;
            }

            public Value makeValue() => new Value<Funqtion>(funqtion, TypeDefinition.Qallable);
        }

        public enum Access
        {
            Public = 1,
            Protected = 2,
            Private = 3
        }

        public struct Args
        {
            public string module;
            public string name;
            public NativeType nativeType;
            public Storage<string, Field> fields;
            public Storage<string, Method> methods;
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

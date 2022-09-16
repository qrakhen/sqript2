using System;
using System.Collections.Generic;

namespace Qrakhen.Sqr.Core
{
    public delegate Value ExtenderFunqtion(Value[] parameters);
    public class ExtenderFunqtionAttribute : Attribute { }

    public class Value : ITyped<Value.Type>
    {
        private readonly Storage<string, ExtenderFunqtion> extensions;
        public readonly bool isPrimitive;

        public Value(Value.Type type = Type.None, bool isPrimitive = false)
        {
            this.type = type;
            this.isPrimitive = isPrimitive;
        }

        [ExtenderFunqtion]
        public Value toString(Value[] parameters)
        {
            return new String(ToString());
        }

        private Type typeFromSysType(System.Type type) // into dict..
        {
            if (type == typeof(bool)) return Type.Boolean;
            if (type == typeof(double)) return Type.Number;
            if (type == typeof(string)) return Type.String;
            if (type == typeof(Qollection)) return Type.Qollection;
            if (type == typeof(Objeqt)) return Type.Objeqt;
            if (type == typeof(Funqtion)) return Type.Funqtion;
            if (type == typeof(Value)) return Type.Variable;
            return Type.None;
        }

        [Flags]
        public enum Type
        {
            None = default,
            Boolean = 1,
            Number = 2,
            String = 4,
            Qollection = 8,
            Objeqt = 16,
            Funqtion = 32,
            Qontext = Qollection | Objeqt | Funqtion,
            Variable = 64
        }
    }

    public class Value<T> : Value
    {
        protected T __value;

        public Value(T value = default(T), Value.Type type = Type.None, bool isPrimitive = false) : base(type, isPrimitive)
        {
            __value = value;
        }

        public override string ToString()
        {
            return __value.ToString();
        }
    }
}

using System;
using System.Collections.Generic;

namespace Qrakhen.Sqr.Core
{
    public delegate Value ExtenderFunqtion(Value[] parameters);
    public class ExtenderFunqtionAttribute : Attribute { }

    public class Value : ITyped<Value.Type>
    {
        public static Value Null => new Value(Type.Null, false);

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

        public override bool Equals(object obj)
        {
            if (type == Type.Null && obj is Value)
                return (obj as Value).type == type;

            return base.Equals(obj);
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
            Variable = 64,
            Null = 128
        }
    }

    public class Value<T> : Value
    {
        protected T __value;

        public Value(T value = default(T), Value.Type type = Type.None, bool isPrimitive = false) : base(type, isPrimitive)
        {
            __value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Value<T>)
                return (obj as Value<T>).__value.Equals(__value);

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return __value.ToString();
        }
    }
}

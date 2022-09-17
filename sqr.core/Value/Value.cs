using System;
using System.Collections.Generic;
using System.Linq;

namespace Qrakhen.Sqr.Core
{
    public class Value
    {
        public static Value Null => null;

        public readonly Storage<string, Variable> fields;
        public readonly TypeDefinition type;

        public Value(TypeDefinition definition)
        {
            if (definition == null)
                definition = TypeDefinition.Value;
            this.type = definition;
        }

        public virtual Value accessMember(string name)
        {
            if (type.methods.contains(name))
                return type.methods[name].makeValue();
            else if (fields.contains(name))
                return fields[name];
            else
                return Null;
        }

        public Value lookAhead(string[] memberNames)
        {
            Value v = this;
            for (int i = 0; i < memberNames.Length; i++)
            {
                v = v.accessMember(memberNames[i]);
                if (v == null)
                    throw new SqrError("could not find name " + memberNames + " in the current qontext (recursive look ahead)");
            }
            return v;
        }

        public virtual String toString()
        {
            return new String("Value"); 
        }
    }

    public class Value<T> : Value
    {
        protected T __value;

        public Value(T value = default(T), TypeDefinition definition = null) : base(definition)
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

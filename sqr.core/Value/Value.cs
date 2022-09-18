using System;
using System.Collections.Generic;
using System.Linq;

namespace Qrakhen.Sqr.Core
{
    public class Value
    {
        public static Value Null => null;

        public virtual object raw => null;
        public virtual Value obj { get => this; }

        public readonly Storage<string, Variable> fields;
        public readonly Type type;

        public Value(Type type)
        {
            if (type == null)
                type = Type.Value;

            this.type = type;
        }

        public virtual Value accessMember(string name)
        {
            if (type.methods.contains(name))
                return type.methods[name].makeValue();
            else if (fields != null && fields.contains(name))
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
                    throw new SqrError("could not find name " + memberNames[i] + " in the current qontext (recursive look ahead)");
            }
            return v;
        }

        public bool isCompatibleType(Value other)
        {
            if (other.type == type) {
                // mit vererbung weitermachen, Type.extends
                return true;
            }
            return false;
        }

        public virtual object getValue()
        {
            return this;
        }

        public override string ToString()
        {
            return type.name + "\nvalue: " + obj + "\nraw: " + raw;
        }

        public virtual String toString()
        {
            return new String(ToString());
        }
    }

    public class Value<T> : Value
    {
        protected T __value;

        public new virtual T raw => __value;
        public override Value obj { get => this; }

        public Value(T value = default(T), Qrakhen.Sqr.Core.Type type = null) : base(type)
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
            return __value?.ToString();
        }
    }
}

using Newtonsoft.Json;

namespace Qrakhen.Sqr.Core
{
    public class Value
    {
        public static readonly Value Null = new Null();
        public static readonly Value Void = new Void();

        public virtual object raw => this;
        public virtual Value obj => this;

        public Storage<string, Variable> fields { get; protected set; }
        [JsonIgnore] public readonly Type type;

        public Value(Type type)
        {
            if (type == null)
                type = Type.Value;

            this.type = type;
        }

        public virtual Value accessMember(Value name)
        {
            string key = name as String;
            if (type.methods.contains(key))
                return type.methods[key].makeQallable(this);
            else if (fields != null && fields.contains(key))
                return fields[key];
            else
                return Null; // throw new SqrTypeError("unknown member " + name + " of type " + type?.name);
        }

        public Value lookAhead(string[] memberNames)
        {
            Value v = this;
            /*for (int i = 0; i < memberNames.Length; i++)
            {
                v = v.accessMember(memberNames[i]);
                if (v == null)
                    throw new SqrError("could not find name " + memberNames[i] + " in the current qontext (recursive look ahead)");
            }*/
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

        public override string ToString()
        {
            return type.name;
        }

        public virtual string toDebugString()
        {
            return ToString();
        }

        [NativeMethod]
        public virtual String toString()
        {
            return new String(ToString());
        }

        [NativeMethod]
        public virtual String getType()
        {            
            return new String(obj == null ? type.render() : obj.type.render()); // ?? i dont even know
        }

        public static implicit operator bool(Value v) { return (v != null); }
    }

    public class Void : Value
    {
        public Void() : base(null) { }
        public override string ToString() => "Void";
    }

    public class Null : Value
    {
        public Null() : base(null) { }
        public override string ToString() => "Null";
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
            if (obj is Value<T> && (obj as Value<T>).__value != null)
                return (obj as Value<T>).__value.Equals(__value);

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return __value?.ToString();
        }

        public override string toDebugString()
        {
            return type.name + "(" + ToString() + ")";
        }
    }
}

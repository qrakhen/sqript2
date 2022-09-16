using System;

namespace Qrakhen.Sqr.Core
{  
    public sealed class Value : ITyped<Value.Type>
    {
        private bool __set;
        private object __value;

        public object rawValue => __value;

        public readonly bool isReference;
        public readonly bool isStrictType;
        public readonly bool isReadonly;

        public Value(
                Type type, 
                object value = null, 
                bool isReference = false, 
                bool isStrictType = false, 
                bool isReadonly = false)
        {
            this.type = type;
            this.isReference = isReference;
            this.isStrictType = isStrictType;
            this.isReadonly = isReadonly;
            if (value != null)
                this.set(value);
        }            

        public void set(object value)
        {
            if (isReadonly && __set)
                throw new SqrError("can not set value of readonly value", this);

            if (isReference && value.GetType() != typeof(Value))
                throw new SqrError("can not assign type " + value.GetType() + " to reference", this);
            
            if (!isReference && value.GetType() == typeof(Value))
                value = (value as Value).rawValue;

            if (isStrictType && type != typeFromSysType(value.GetType()))
                throw new SqrError("can not assign type of " + value + " to type of " + type, this);
            else
                type = typeFromSysType(value.GetType());

            __value = value;

            if (!__set)
                __set = true;
        }

        public T get<T>()
        {
            if (isReference)
                return (rawValue as Value).get<T>();
            else
                return (T)rawValue;
        }

        public Value getReference()
        {
            if (isReference)
                return (Value)rawValue;
            else
                throw new SqrError("value is not a reference", this);
        }

        public double asNumber() => get<double>();
        public bool asBoolean() => get<bool>();

        public override string ToString()
        {
            return rawValue == null ? "null" : rawValue.ToString();
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
            Reference = 64
        }

        public bool isTypeDefaultReferenced(Type type)
        {
            return (!isType(Type.Boolean | Type.Number | Type.String));
        }

        private Type typeFromSysType(System.Type type) // into dict..
        {
            if (type == typeof(bool)) return Type.Boolean;
            if (type == typeof(double)) return Type.Number;
            if (type == typeof(string)) return Type.String;
            if (type == typeof(Qollection)) return Type.Qollection;
            if (type == typeof(Objeqt)) return Type.Objeqt;
            if (type == typeof(Funqtion)) return Type.Funqtion;
            if (type == typeof(Value)) return Type.Reference;
            return Type.None;
        }
    }
}

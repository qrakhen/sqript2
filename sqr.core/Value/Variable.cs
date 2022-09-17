using System;

namespace Qrakhen.Sqr.Core
{  
    public sealed class Variable : Value<Value>
    {
        private bool __set;

        public Value value { get => get(); set => set(value); }

        public new TypeDefinition definition => value?.definition;

        public bool isReference { get; private set; }
        public readonly bool isStrictType;
        public readonly bool isReadonly;

        public Variable(
                Value value = null, 
                bool isReference = false, 
                bool isStrictType = false, 
                bool isReadonly = false) : base(value, value?.definition ?? null)
        {
            this.isReference = isReference;
            this.isStrictType = isStrictType;
            this.isReadonly = isReadonly;
            if (value != null)
                set(value);
        }

        public void set(Value value, bool asReference = false)
        {
            if (isReadonly && __set)
                throw new SqrError("can not set value of readonly value", this);

            // mutate to real value
            if (!asReference && value.GetType() == typeof(Variable))
                value = (value as Variable).__value;

            // typecheck
            if (isStrictType && nativeType != value.nativeType)
                throw new SqrError("can not assign type of " + value + " to type of " + nativeType, this);

            // reference logic
            if (asReference) {
                if (!isReference) {
                    if (isStrictType)
                        throw new SqrError("can not make value into reference due to it being strictly typed to " + nativeType, this);
                    else
                        isReference = true;
                }

                if (value.GetType() != typeof(Variable))
                    throw new SqrError("can not assign type " + value.GetType() + " as reference, has to be an identifier or name", this);

                __value = value; // set value as reference

            } else {
                if (isReference) {
                    if (getReference() == null)
                        throw new SqrError("no reference assigned yet. use <& to assign a value by its reference.", this);
                    getReference().set(value); // set value of reference
                } else {
                    __value = value; // set value as real value
                }
            }         

            if (!__set)
                __set = true;
        }

        public T get<T>()
        {
            return (T)(object)get();
        }

        public Value get()
        {
            if (isReference)
                return (__value as Variable).get();
            else
                return __value;
        }

        public Variable getReference()
        {
            if (isReference)
                return (Variable)__value;
            else
                throw new SqrError("value is not a reference", this);
        }

        public override string ToString()
        {
            return __value == null ? "null" : __value.ToString();
        }

        public bool isTypeDefaultReferenced(NativeType type)
        {
            return (!isType(NativeType.Boolean | NativeType.Number | NativeType.String));
        }
    }
}

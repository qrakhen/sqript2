using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public abstract class ItemSet : Value
    {
        [NativeField]
        public abstract int length { get; }

        [NativeField]
        public Type itemType { get; protected set; }

        public ItemSet(Type type) : base(type) { }

        [NativeMethod]
        public abstract void set(Value index, Value value);

        [NativeMethod]
        public abstract Value get(Value index);
    }
}

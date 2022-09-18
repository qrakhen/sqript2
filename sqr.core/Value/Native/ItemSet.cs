using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public abstract class ItemSet : Value
    {
        [Native]
        public abstract int length { get; }

        [Native]
        public Type itemType { get; protected set; }

        public ItemSet(Type type) : base(type) { }

        [Native]
        public abstract void set(Value index, Value value);

        [Native]
        public abstract Value get(Value index);
    }
}

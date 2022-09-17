using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public abstract class Qollection : Value
    {
        [Native]
        public int length { get; protected set; }

        [Native]
        public Type itemType { get; protected set; }

        public Qollection(Type type) : base(type) { }

        [Native]
        public abstract void set(Number index, Value value);

        [Native]
        public abstract Value get(Number index);
    }
}

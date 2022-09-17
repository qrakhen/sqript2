using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Qollection : ItemSet
    {
        protected List<Value> items = new List<Value>();
        public override int length => items.Count;
        public Type itemType { get; protected set; }

        public Qollection() : base(Type.Qollection)
        {

        }

        [Native]
        public void add(Value value)
        {
            items.Add(value);
        }

        [Native]
        public override Value get(Number index)
        {
            return items[index.asInteger()];
        }

        [Native]
        public override void set(Number index, Value value)
        {
            items[index.asInteger()] = value;
        }
    }
}

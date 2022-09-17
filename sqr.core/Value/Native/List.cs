using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class List : Qollection
    {
        protected List<Value> items;

        public List() : base(Type.List)
        {
        }

        [Native]
        public void add(Value value)
        {
            items.Add(value);
        }

        public override Value get(Number index)
        {
            return items[index.asInteger()];
        }

        public override void set(Number index, Value value)
        {
            items[index.asInteger()] = value;
        }
    }
}

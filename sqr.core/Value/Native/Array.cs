using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Array : ItemSet
    {
        public override int length => items.Length;
        protected Value[] items;

        public Array() : base(Type.Array)
        {
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

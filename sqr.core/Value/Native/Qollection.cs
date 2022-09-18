using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override Value accessMember(string name)
        {
            var member = base.accessMember(name);
            if (member == Null) {
                var index = Convert.ToInt32(name);
                if (items.Count > index && index > 0)
                    return items[index];
                else
                    throw new SqrError("index " + index + " outside of Qollection's boundaries");
            }
            return member;
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

        public override string ToString()
        {
            return "[\n" + string.Join("\n", items?.Select(_ => "    " + _?.ToString())) + "\n]"; 
        }
    }
}

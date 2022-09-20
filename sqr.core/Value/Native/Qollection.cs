using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Qollection : ItemSet
    {
        [NativeField] public List<Value> items = new List<Value>();
        [NativeField] public override int length => items.Count;

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

        [NativeMethod]
        public void add(Value[] parameters) => add(parameters[0]);

        public void add(Value value)
        {
            items.Add(value);
        }

        [NativeMethod]
        public override Value get(Value index)
        {
            return items[(index as Number).asInteger()];
        }

        [NativeMethod]
        public override void set(Value index, Value value)
        {
            items[(index as Number).asInteger()] = value;
        }

        public override string ToString()
        {
            var r = "[\n";
            var index = 0;
            foreach (var i in items) {
                r += "    " + (index++).ToString() + ": " + i.ToString() + "\n";
            }
            return r + "]";
        }
    }
}

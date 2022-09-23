using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Array : ItemSet
    {
        public override int length => items.Length;
        public Variable[] items;

        public Array() : base(Type.Array)
        {
        }

        public override Value accessMember(string name)
        {
            var member = base.accessMember(name);
            if (member == Null) {
                var index = Convert.ToInt32(name);
                if (items.Length > index && index > 0)
                    return items[index];
                else
                    throw new SqrError("index " + index + " outside of Qollection's boundaries");
            }
            return member;
        }

        [NativeMethod]
        public override Value get(Value index)
        {
            return items[(index as Number).asInteger()];
        }

        [NativeMethod]
        public override void set(Value index, Value value)
        {
            items[(index as Number).asInteger()] = new Variable(value);
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

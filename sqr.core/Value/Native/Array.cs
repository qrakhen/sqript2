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

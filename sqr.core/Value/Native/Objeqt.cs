using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Objeqt : ItemSet
    {
        public Storage<string, Variable> properties = new Storage<string, Variable>();

        public override int length => properties.count;

        public Objeqt() : base(Type.Objeqt)
        {

        }

        public override Value accessMember(string name)
        {
            var member = base.accessMember(name);
            if (member == Null) {
                if (properties.contains(name))
                    return properties[name];
                else
                    return null;
            }
            return member;
        }

        public override Value get(Value index)
        {
            return properties[(string)index.raw].obj;
        }

        public override void set(Value index, Value value)
        {
            properties[(string)index.raw].set(value);
        }

        public override string ToString()
        {
            var r = "{\n";
            foreach (var p in properties) {
                r += "    " + p.Key + ": " + p.Value.toString() + "\n";
            }
            return r + "}";
        }
    }
}

using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Objeqt : ItemSet
    {
        public Storage<string, Variable> properties = new Storage<string, Variable>();

        [NativeField]
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

        [NativeMethod]
        public override Value get(Value index)
        {
            return properties[(string)index.raw].obj;
        }

        [NativeMethod]
        public override void set(Value index, Value value)
        {
            properties[(string)index.raw].set(value);
        }

        public override string ToString()
        {
            return render();
        }

        public string render(int __level = 0)
        {
            var ident = "".PadLeft(__level * 2);
            var r = ident + "{\n";
            foreach (var p in properties) {
                var v = p.Value.ToString();
                if (p.Value.obj is Objeqt)
                    v = (p.Value.obj as Objeqt).render(__level++);
                r += ident + "  " + p.Key + ": " + v + "\n";
            }
            return r + ident + "}";
        }
    }
}

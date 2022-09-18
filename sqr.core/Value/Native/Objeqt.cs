using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Objeqt : Value
    {
        private readonly Storage<string, Value> properties = new Storage<string, Value>();

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
    }
}

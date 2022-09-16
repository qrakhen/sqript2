using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{  
    public class Qontext : Value
    {
        [JsonProperty]
        protected Storage<string, Variable> names = new Storage<string, Variable>();

        public static readonly Qontext globalContext = new Qontext();

        public Qontext parent { get; protected set; }

        public Qontext(Qontext parent = null) : base(Value.Type.Qontext, false)
        {
            this.parent = parent;
        }

        public Value get(string name)
        {
            return names[name];
        }

        public Value register(
                Token token, 
                Value.Type type = Value.Type.None, 
                Value value = null, 
                bool isReference = false, 
                bool isStrictType = false, 
                bool isReadonly = false)
        {
            var name = token.get<string>();
            if (names[name] != null) {
                throw new SqrError("name " + name + " already declared in qontext");
            }
            return names[name] = new Variable(value, isReference, isStrictType, isReadonly);
        }

        public Value resolveName(string name) => resolveName(new string[] { name });

        public Value resolveName(string[] name)
        {
            var qontext = lookUp(name[0]);
            return qontext.lookAhead(name);
        }

        protected Qontext lookUp(string name)
        {
            if (names.ContainsKey(name))
                return this;
            else if (parent != null)
                return parent.lookUp(name);
            else
                throw new SqrError("could not find the name " + name[0] + " within the current qontext (recursive lookup)");
        }

        protected Value lookAhead(string[] name)
        {
            Qontext q = this;
            for (int i = 0; i < name.Length - 1; i++) {
                var v = q.get(name[i]);
                if (v is Qontext)
                    q = v as Qontext;
                else
                    throw new SqrError("could not find name " + name + " in the current qontext (recursive look ahead)");
            }

            return (q != null ? q.get(name[name.Length - 1]) : null);
        }
    }
}

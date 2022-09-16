using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Variable get(string name)
        {
            return names[name];
        }

        public Variable register(
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

        public Variable resolveName(string name) => resolveName(new string[] { name });

        public Variable resolveName(string[] name)
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

        protected Variable lookAhead(string[] name)
        {
            Qontext q = this;
            for (int i = 0; i < name.Length - 1; i++) {
                var v = q.get(name[i])?.get();
                if (v is Qontext)
                    q = v as Qontext;                
                else if (v is Value) {
                    // native function/extensions not final
                    var m = v.GetType().GetMethod(name[i + 1]);
                    m.Invoke(v, new Value[0]);
                } else
                    throw new SqrError("could not find name " + name + " in the current qontext (recursive look ahead)");
            }

            return (q != null ? q.get(name[name.Length - 1]) : null);
        }
    }
}

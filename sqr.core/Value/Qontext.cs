using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{  
    public class Qontext
    {
        [JsonProperty]
        protected Storage<string, Value> names = new Storage<string, Value>();

        public static readonly Qontext globalContext = new Qontext();

        public Qontext parent { get; protected set; }

        public Qontext(Qontext parent = null)
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
                object value = null, 
                bool isReference = false, 
                bool isStrictType = false, 
                bool isReadonly = false)
        {
            var name = token.get<string>();
            if (names[name] != null) {
                throw new SqrError("name " + name + " already declared in qontext");
            }
            return names[name] = new Value(type, value, isReference, isStrictType, isReadonly);
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
                if (v.isType(Value.Type.Qontext))
                    q = v.get<Qontext>();
                else
                    throw new SqrError("could not find name " + name + " in the current qontext (recursive look ahead)");
            }

            return (q != null ? q.get(name[name.Length - 1]) : null);
        }
    }

    public class Qollection : Qontext
    {
        // override names with indexes
        public void add(Value value) { }
    }

    public class Objeqt : Qontext
    {

    }

    public class Funqtion : Qontext
    {
        public Parameter[] declaredParameters;
        public Value.Type returnType;
        public Body body;
        
        public Value execute(Value[] parameters)
        {
            for (int i = 0; i < declaredParameters.Length; i++) {
                var p = declaredParameters[i];
                names[p.name] = parameters[i];
            }

            return null;
        }

        public struct Parameter
        {
            public string name;
            public Value.Type type;
            public Value defaultValue;
            public bool optional;
        }
    }
}

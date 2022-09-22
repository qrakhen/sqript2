using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Qontext
    {
        [JsonProperty]
        public readonly Storage<string, Variable> names = new Storage<string, Variable>();
        public readonly Storage<string, Value> exports = new Storage<string, Value>();
        public readonly Storage<string, Value> imports = new Storage<string, Value>();

        public static readonly Qontext globalContext = new Qontext();

        public Qontext parent { get; protected set; }

        public Qontext(Qontext parent = null)
        {
            this.parent = parent;
        }

        public Variable get(string name)
        {
            return names[name];
        }

        public Variable register(
                string name, 
                Value value = null, 
                bool isReference = false, 
                Type strictType = null, 
                bool isReadonly = false)
        {
            if (names[name] != null) {
                Logger.TEMP_STATIC_DEBUG.warn("warning: overwriting existing name " + name + " in qontext (make this configurable)");
                //throw new SqrError("name " + name + " already declared in qontext");
            }
            return names[name] = new Variable(value, isReference, strictType, isReadonly);
        }

        public void export(
                string name,
                Value value)
        {
            if (exports[name] != null) {
                throw new SqrQontextError("name " + name + " already exported");
            }
            exports[name] = value;
        }

        public void import(
                Qontext qontext)
        {
            foreach (var e in qontext.exports) {
                if (imports[e.Key] != null) {
                    throw new SqrQontextError("name " + e.Key + " already imported!");
                }
                if (names[e.Key] != null) {
                    throw new SqrQontextError("name " + e.Key + " already defined in local qontext!");
                }
                imports[e.Key] = e.Value;
            }
        }

        public Value resolveName(string name) => resolveName(new string[] { name });

        public Value resolveName(string[] name)
        {
            var qontext = lookUp(name[0]);
            var value = qontext.get(name[0]);
            if (name.Length > 1)
                return value.lookAhead(name.AsSpan(1).ToArray());
            return value;
        }

        public Qontext lookUp(string name)
        {
            if (names.ContainsKey(name))
                return this;
            else if (parent != null)
                return parent.lookUp(name);
            else
                throw new SqrError("could not find the name " + name + " within the current qontext (recursive lookup)");
        }
    }
}

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
        public readonly Storage<string, Module> imports = new Storage<string, Module>();

        private readonly Module __module;
        public Module module => __module == null ? parent.module : __module;

        public static readonly Qontext globalContext = new Qontext();

        public Qontext parent { get; protected set; }

        public Qontext(Qontext parent = null, Module module = null)
        {
            this.parent = parent;
            this.__module = module;
        }

        public Value get(string name)
        {
            if (names.contains(name))
                return names[name];
            else if (imports.contains(name))
                return imports[name];
            return null;
        }

        public Variable register(
                string name, 
                Value value = null, 
                bool isReference = false, 
                Type strictType = null, 
                bool isReadonly = false)
        {
            if (names[name] != null) {
                if (!Runtime.qonfig.overwriteExistingNames)
                    throw new SqrError("name " + name + " already declared in qontext");
                Logger.TEMP_STATIC_DEBUG.warn("warning: overwriting existing name " + name + " in qontext (make this configurable)");
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

        public Value resolveName(Value name) => resolveName(new Value[] { name });

        public Value resolveName(Value[] name)
        {
            var qontext = lookUp(name[0] as String);
            var value = qontext.get(name[0] as String);
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

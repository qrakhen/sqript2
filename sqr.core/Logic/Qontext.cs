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
        public readonly Storage<string, Value> names = new Storage<string, Value>();
        public readonly Storage<string, Type> types = new Storage<string, Type>();
        public readonly Storage<string, Module> imports = new Storage<string, Module>();

        private readonly Module __module;
        public Module module => __module == null ? parent.module : __module;

        public static readonly Qontext globalContext = new Qontext();

        public Qontext parent { get; protected set; }

        public Qontext(Qontext parent = null, Module module = null)
        {
            this.parent = parent;
            __module = module;

            if (module) {
                import(CoreModule.instance);
                Logger.TEMP_STATIC_DEBUG.debug("New Qontext in module: " + module.name);
            }
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
            var v = new Variable(value, isReference, strictType, isReadonly);
            names[name] = v;
            return v;
        }

        public Value register(
                string name,
                Value value)
        {
            if (names[name] != null) {
                if (!Runtime.qonfig.overwriteExistingNames)
                    throw new SqrError("name " + name + " already declared in qontext");
                Logger.TEMP_STATIC_DEBUG.warn("warning: overwriting existing name " + name + " in qontext (make this configurable)");
            }
            return names[name] = value;
        }

        public void import(Module module, string asName = null)
        {
            var name = asName ?? module.name;
            if (imports.contains(name)) 
                throw new SqrQontextError("module with name " + name + " already imported. use import <module> as <name> syntax to create an alias for duplicate names.", this, module);
            
            register(name, module);
            imports[name] = module;
        }

        public Qlass declareType(Type type)
        {
            if (types.contains(type.name))
                throw new SqrQontextError("type " + type + " already declared in qontext!", this, type);

            var qlass = new Qlass(type);
            register(type.name, qlass);
            types[type.name] = type;
            return qlass;
        }

        public void export(Value value, string asName = null)
        {
            module?.export(value, asName);
        }

        public Type resolveType(string name, bool doThrow = true)
        {
            var v = resolveName(name, doThrow);
            if (v != null && v is Qlass) {
                return (v as Qlass).raw;
            }
            return null;
        }

        public Value resolveName(string name, bool doThrow = true)
        {
            Value value = null;
            var qontext = lookUp(name);
            if (qontext != null) {
                value = qontext.names[name];                
            }

            if (!value) {
                var r = imports
                    .findAll(_ => _.get(name))
                    .Select(_ => _.get(name))
                    .Concat(imports.findAll(_ => _.name == name))
                    .ToArray();
                if (doThrow && r.Length == 0)
                    throw new SqrQontextError("could not find the name " + name + " within the current qontext or any imported modules (recursive lookup)", this);
                if (doThrow && r.Length > 1)
                    throw new SqrQontextError(name + " is an ambigious name between several modules: " + string.Join(", ", r.Select(_ => _.toDebugString())), this);
                else if (r.Length == 1)
                    return r[0];
            }

            return value;
        }

        private Qontext lookUp(string name)
        {
            if (names.ContainsKey(name))
                return this;
            else if (parent != null)
                return parent.lookUp(name);
            else
                return null;
        }
    }
}

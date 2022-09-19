﻿using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{  
    public class Qontext
    {
        [JsonProperty]
        protected Storage<string, Variable> names = new Storage<string, Variable>();

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
                bool isStrictType = false, 
                bool isReadonly = false)
        {
            if (names[name] != null) {
                throw new SqrError("name " + name + " already declared in qontext");
            }
            return names[name] = new Variable(value, isReference, isStrictType, isReadonly);
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
                throw new SqrError("could not find the name " + name[0] + " within the current qontext (recursive lookup)");
        }
    }
}
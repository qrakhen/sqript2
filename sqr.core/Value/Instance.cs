using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Instance : Value
    {
        // this is basically just so that we can type "x" rather than "this:x" in funqtions
        public readonly Qontext qontext;
         
        public Instance(Qontext qontext, Type definition) : base(definition)
        {
            this.fields = new Storage<string, Variable>();
            this.qontext = new Qontext(qontext);
        }
    }
}

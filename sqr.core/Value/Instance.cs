using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Instance : Value
    {
        public readonly Qontext qontext;
         
        public Instance(Qontext qontext, Type definition) : base(definition)
        {
            this.qontext = new Qontext(qontext);
        }
    }
}

using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Qollection : Value
    {
        public Qollection() : base(TypeDefinition.Qollection)
        {
        }

        public void add(Value value) { }
    }
}

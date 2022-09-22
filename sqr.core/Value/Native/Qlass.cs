using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Qlass : Value<Type>
    {
        public Qlass(Type type) : base(type, Type.Qlass) 
        {
            
        }

        public override string ToString()
        {
            return __value?.name;
        }

        public override string toDebugString()
        {
            return "Qlass [" + ToString() + "]";
        }
    }
}

using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Boolean : Value<bool>
    {
        public Boolean(bool value) : base(value, Type.Boolean)
        {

        }

        public override string toDebugString()
        {
            return type.name + "(" + ToString() + ")";
        }

        public static implicit operator bool(Boolean b) => b.__value;
        public static implicit operator Boolean(bool b) => new Boolean(b);
    }
}

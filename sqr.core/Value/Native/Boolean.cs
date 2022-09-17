using Newtonsoft.Json;
using Qrakhen.Dependor;
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

        public static implicit operator bool(Boolean b) => b.__value;
        public static explicit operator Boolean(bool b) => new Boolean(b);
    }
}

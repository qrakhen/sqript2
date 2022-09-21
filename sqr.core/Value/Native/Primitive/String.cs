using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class String : Value<string>
    {
        public String(string value) : base(value, Type.String)
        {
            
        }

        [NativeMethod]
        public Number length()
        {
            return new Number(__value?.Length ?? 0);
        }

        [NativeMethod]
        public String span(Value from, Value to)
        {
            return new String(__value.Substring(
                (int)(from as Number),
                (int)(to as Number)));
        }

        public override string ToString()
        {
            return "'" + __value?.ToString() + "'";
        }

        public static implicit operator string(String s) => s.__value;
        public static implicit operator String(string s) => new String(s);
    }
}

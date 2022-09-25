using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class String : Value<string>
    {
        public String(string value = null) : base(value, Type.String)
        {
            
        }

        public override Value accessMember(Value key)
        {
            if (key is Number) {
                int index = (key as Number).asInteger();
                if (index > 0 && __value?.Length > index)
                    return new String(__value[index].ToString());
                else
                    throw new SqrParameterError("index " + index + " outside of string's boundaries (" + __value + ")");
            }
            return base.accessMember(key);
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
            return __value?.ToString();
        }

        public static implicit operator string(String s) => s.__value;
        public static implicit operator String(string s) => new String(s);
    }
}

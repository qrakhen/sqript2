using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class String : Value<string>
    {
        public String(string value) : base(value, Value.Type.String, true)
        {

        }


        public static implicit operator string(String s) => s.__value;
        public static explicit operator String(string s) => new String(s);
    }
}

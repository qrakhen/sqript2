using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Number : Value<double>
    {
        public Number(double value) : base(value, Type.Number)
        {

        }

        public double asDouble() => get();
        public float asFloat() => (float)get();
        public int asInteger() => (int)get();

        public static implicit operator double(Number n) => n.__value;
        public static explicit operator Number(double d) => new Number(d);
    }
}

﻿using Newtonsoft.Json;
using Qrakhen.SqrDI;
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

        public double asDouble() => raw;
        public float asFloat() => (float)raw;
        public int asInteger() => (int)raw;

        public static implicit operator double(Number n) => n.__value;
        public static implicit operator Number(double d) => new Number(d);
    }
}
using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Integer : Value<int>
    {
        public Integer(int value) : base(value, Type.Integer)
        {

        }

        public static implicit operator int(Integer n) => n.__value;
        public static implicit operator Integer(int d) => new Integer(d);
    }
}

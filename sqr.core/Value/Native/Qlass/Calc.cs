using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Calc : Value
    {
        public Calc() : base(CoreModule.instance.getType("Calc"))
        {

        }

        [NativeMethod]
        public static Number round(Number value)
        {
            return new Number(Math.Round(value));
        }

        [NativeMethod]
        public static Number sqrt(Number value)
        {
            return new Number(Math.Sqrt(value));
        }

        [NativeMethod]
        public static Number abs(Number value)
        {
            return new Number(Math.Abs(value));
        }
    }
}

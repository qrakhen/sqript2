using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Parse : Value
    {
        public Parse() : base(CoreModule.instance.getType("Parse"))
        {

        }

        [NativeMethod]
        public static Number asInt(Value value)
        {
            return int.Parse(value.raw?.ToString());
        }
    }
}

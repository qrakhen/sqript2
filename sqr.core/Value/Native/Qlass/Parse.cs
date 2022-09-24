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
        public Parse() : base(Type.get("Parse"))
        {

        }

        [NativeMethod]
        public static Number asInt(Value value)
        {
            return int.Parse(value.raw?.ToString());
        }

        static Parse()
        {
            Type.register(typeof(Parse), new Type.Args {
                name = "Parse",
                module = Type.coreModule,
                extends = Type.Value,
                nativeType = NativeType.Static
            });
        }
    }
}

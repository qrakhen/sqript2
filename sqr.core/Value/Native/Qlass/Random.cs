using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Random : Value<System.Random>
    {       
        private static Random staticInstance = new Random();

        public Random() : base(new System.Random(), CoreModule.instance.getType("Random"))
        {

        }

        [NativeMethod]
        public Number next()
        {
            return new Number(__value.NextDouble());
        }

        [NativeMethod]
        public Number range(Number from, Number to)
        {
            return new Number(from + ((to - from) * next()));
        }

        [NativeMethod]
        public static Number random()
        {
            return new Number(staticInstance.next());
        }
    }
}

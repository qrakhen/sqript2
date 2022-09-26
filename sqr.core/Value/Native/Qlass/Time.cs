using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Time : Value<DateTime>
    {
        public const int WIN_TICKS = 10000000;
        public const long SEC_TO_UNIX_EPOCH = 11644473600L;

        public Time(DateTime value) : base(value, CoreModule.instance.getType("Time")) { }
        public Time() : base(DateTime.Now, CoreModule.instance.getType("Time"))
        {

        }

        [NativeMethod]
        public Number time()
        {
            return new Number(__value.ToFileTimeUtc());
        }

        [NativeMethod]
        public Number unixTime()
        {
            return new Number(__value.ToFileTimeUtc() / WIN_TICKS - SEC_TO_UNIX_EPOCH);
        }

        [NativeMethod]
        public String format(String format)
        {
            var r = format.raw
                .Replace("Y", __value.Year.ToString())
                .Replace("y", __value.Year.ToString().Substring(2))
                .Replace("m", __value.Month.ToString())
                .Replace("d", __value.Day.ToString())
                .Replace("h", __value.Hour.ToString())
                .Replace("i", __value.Minute.ToString())
                .Replace("s", __value.Second.ToString());
            return new String(r);
        }

        public static implicit operator DateTime(Time t) => t.__value;
        public static implicit operator Time(DateTime t) => new Time(t);
    }
}

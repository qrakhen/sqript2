using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Qonsole : Value
    {       
        public Qonsole() : base(CoreModule.instance.getType("Qonsole"))
        {

        }

        [NativeMethod]
        public static void write(Value input)
        {
            Console.Write((string)input.toString());
        }

        [NativeMethod]
        public static void writeLine(Value input)
        {
            Console.WriteLine((string)input.toString());
        }

        [NativeMethod]
        public static Number read()
        {
            return new Number(Console.Read());
        }

        [NativeMethod]
        public static String readLine()
        {
            return new String(Console.ReadLine());
        }
    }
}

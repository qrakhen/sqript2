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
        public Qonsole() : base(Type.get("Qonsole"))
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

        static Qonsole()
        {
            Type.register(typeof(Qonsole), new Type.Args {
                name = "Qonsole",
                module = Type.coreModule,
                extends = Type.Value,
                nativeType = NativeType.Static
            });
        }
    }
}

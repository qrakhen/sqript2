using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Qrakhen.Sqr.Core
{
    public class Qonsole : Value
    {       

        private static ConsoleColor _consoleColor = ConsoleColor.White;

		public Qonsole() : base(Type.get("Qonsole"))
        {

        }

		[NativeMethod]
		public static void setColor(String color) {
            if (Enum.TryParse(typeof(ConsoleColor), color, true, out object newColor)) {
				Console.ForegroundColor = (ConsoleColor) newColor;
			}
		}

		[NativeMethod]
        public static void write(Value input) 
		{
			writeC(input, "");
		}

		[NativeMethod]
		public static void writeC(Value input, String color) {
			ConsoleColor oldColor = Console.ForegroundColor;
			if (Enum.TryParse(typeof(ConsoleColor), color, true, out object newColor)) {
				Console.ForegroundColor = (ConsoleColor) newColor;
				Console.Write(Regex.Unescape((string) input.toString()));
				Console.ForegroundColor = oldColor;
			} else {
				Console.ForegroundColor = _consoleColor;
				Console.Write(Regex.Unescape((string) input.toString()));
				Console.ForegroundColor = oldColor;
			}
		}

		[NativeMethod]
		public static void writeLine(Value input) {
			writeLineC(input, "");
		}

		[NativeMethod]
        public static void writeLineC(Value input, String color) {
			ConsoleColor oldColor = Console.ForegroundColor;
			if (Enum.TryParse(typeof(ConsoleColor), color, true, out object newColor)) {
				Console.ForegroundColor = (ConsoleColor) newColor;
				Console.WriteLine(Regex.Unescape((string) input.toString()));
				Console.ForegroundColor = oldColor;
			} else {
				Console.ForegroundColor = _consoleColor;
				Console.WriteLine(Regex.Unescape((string) input.toString()));
				Console.ForegroundColor = oldColor;
			}
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

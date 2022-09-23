using System;
using Qrakhen.Sqr.Core;
using System.IO;

namespace Qrakhen.Sqr.shell
{
    class Program
    {
		private static void t(int condition)
        {
			bool a = false;
			bool b = false;
			bool c = false;

			if (condition == 0) {
				a = true;
            } else if (condition == 1) {
				b = true;
            } /*else {
				c = true;
            }*/

			bool final = (a || b || c);
			Console.Write(final);
        }

		static void Main(string[] args) 
		{
			t(0);
			t(1);
			t(2);

			Console.ForegroundColor = ConsoleColor.White;
			if (args.Length == 0) {
				SqrDI.Dependor.get<Runtime>().run();
			} else {
				if (args[1] == "--version") {
					Console.WriteLine("Sqript v" + Runtime.version);
				} else {
					var content = File.ReadAllText(args[0] + (args[0].EndsWith(".sq") ? "" : ".sq"));
					SqrDI.Dependor.get<Runtime>().run(content);
				}
			}
		}
	}
}

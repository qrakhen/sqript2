using System;
using Qrakhen.Sqr.Core;
using System.IO;

namespace Qrakhen.Sqr.shell
{
    class Program
    {	
		static void Main(string[] args) 
		{
			Console.ForegroundColor = ConsoleColor.White;
			if (args.Length == 0) {
				SqrDI.Dependor.get<Runtime>().run();
			} else {
				if (args[1] == "--version") {
					Console.WriteLine("Sqript v" + Runtime.version);
				} else {
					SqrDI.Dependor.get<Runtime>().run(args[0] + (args[0].EndsWith(".sq") ? "" : ".sq"));
				}
			}
		}
	}
}

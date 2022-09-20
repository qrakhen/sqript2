using System;
using Qrakhen.Sqr.Core;
using System.Linq;
using System.Collections.Generic;

namespace Qrakhen.Sqr.shell
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            SqrDI.Dependor.get<Runtime>().run();
        }
    }
}

using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class UCI
    {
        private readonly Logger log;
        private readonly Runtime runtime;

        public static string prefix = "    <: ";

        private Thread thread;
        private Stopwatch clock;
        
        public int exitCode { get; private set; } = 0;

        public void run()
        {
            clock = new Stopwatch();
            thread = new Thread(__run);
            clock.Start();
            thread.Start();
        }

        private void __run()
        {
            ConsoleKeyInfo keyInfo;
            
            do {
                string input = "", drawn = prefix;
                while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter) {
                    if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0) {
                        input = input.Substring(0, input.Length - 1);
                        clearLine();
                        write(input);
                    } else if (keyInfo.Key == ConsoleKey.Tab) {
                        var match = Qontext.globalContext.names.Keys
                            .FirstOrDefault(item => item != input && item.StartsWith(input, true, CultureInfo.InvariantCulture));
                        if (string.IsNullOrEmpty(match))
                            continue;

                        clearLine();
                        input = "";
                        write(match);
                    } else {
                        input += keyInfo.KeyChar;
                        write(keyInfo.KeyChar.ToString());
                    }
                }
                if ((keyInfo.Modifiers & ConsoleModifiers.Shift) != ConsoleModifiers.Shift) {
                    runtime.execute(input);
                } else {
                    input += "\n";
                    write(keyInfo.KeyChar.ToString());
                }
            } while (exitCode == 0);
        }

        private void clearLine()
        {
            var c = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(prefix + new string(' ', Console.WindowWidth - prefix.Length));
            Console.SetCursorPosition(prefix.Length, c);
        }

        // das alles in eine console klasse
        private void write(string input)
        {
            Console.Write(input);
        }

        private string doTheConsoleThing()
        {
            var builder = new StringBuilder();
            var input = Console.ReadKey(true);

            while ((input = Console.ReadKey(true)).Key != ConsoleKey.Enter) {
                var c = builder.ToString();
                if (input.Key == ConsoleKey.Tab) {
                    var match = Qontext.globalContext.names.Keys
                        .FirstOrDefault(item => item != c && item.StartsWith(c, true, CultureInfo.InvariantCulture));
                    if (string.IsNullOrEmpty(match)) {
                        continue;
                    }

                    clearLine();
                    builder.Clear();
                    Console.Write("    <:" + match);
                    builder.Append(match);
                } else {
                    if (input.Key == ConsoleKey.Backspace && c.Length > 0) {
                        builder.Remove(builder.Length - 1, 1);
                        clearLine();

                        c = c.Remove(c.Length - 1);
                        Console.Write("    <:" + c);
                    } else {
                        var key = input.KeyChar;
                        builder.Append(key);
                        Console.Write(key);
                    }
                }
            }
           
            return builder.ToString();
        }       

        private string json(object any)
        {
            return JsonConvert.SerializeObject(
                any,
                Formatting.Indented,
                new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    MaxDepth = 1
                }
            );
        }
    }
}

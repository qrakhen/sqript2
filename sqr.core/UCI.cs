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
        public static readonly Storage<ConsoleColor, char> colors = new Storage<ConsoleColor, char>() {
            { ConsoleColor.White, '☺' }, // ☺
            { ConsoleColor.Black, '☻' },
            { ConsoleColor.Red, '♥' },
            { ConsoleColor.Green, '♦' },
            { ConsoleColor.Blue, '♣' },
            { ConsoleColor.Cyan, '♠' },
            { ConsoleColor.Yellow, '•' },
            { ConsoleColor.Gray, '◘' },
            { ConsoleColor.DarkRed, '○' },
            { ConsoleColor.DarkGreen, '◙' },
            { ConsoleColor.DarkBlue, '♀' },
            { ConsoleColor.DarkCyan, '♪' },
            { ConsoleColor.DarkYellow, '♫' },
            { ConsoleColor.DarkGray, '☼' }
        };

        public static readonly Storage<Token.Type, ConsoleColor> mapping = new Storage<Token.Type, ConsoleColor>() {
            { Token.Type.Identifier, ConsoleColor.White },
            { Token.Type.Boolean, ConsoleColor.DarkYellow },
            { Token.Type.Operator, ConsoleColor.DarkRed },
            { Token.Type.Number, ConsoleColor.Yellow },
            { Token.Type.Type, ConsoleColor.Green },
            { Token.Type.TypeValue, ConsoleColor.DarkGreen },
            { Token.Type.Keyword, ConsoleColor.Blue },
            { Token.Type.Accessor, ConsoleColor.DarkGray },
            { Token.Type.String, ConsoleColor.Cyan },
            { Token.Type.Structure, ConsoleColor.DarkGray },
            { Token.Type.End, ConsoleColor.DarkGray }
        };

        private const string HISTORY_FILE = "sqr.history";

        private readonly Logger log;
        private readonly Runtime runtime;
        private readonly TokenResolver tokenResolver;

        public static string prefix = "    <: ";

        private Thread thread;
        private Stopwatch clock;
        private int historyIndex = 0;
        private List<char> line = new List<char>();
        private List<char> buffer = new List<char>();
        private List<char> chars => buffer.Concat(line).ToList();
        private string input => new string(chars.ToArray());

        private int cx => Console.CursorLeft - prefix.Length;
        private int cy => Console.CursorTop;

        public List<string> history { get; private set; } = new List<string>();
        public int exitCode { get; private set; } = 0;

        public void run()
        {
            if (!File.Exists(HISTORY_FILE)) {
                File.Create(HISTORY_FILE);
            } else {
                history = File.ReadAllText(HISTORY_FILE).Split("\n").ToList();
            }
            historyIndex = history.Count;
            clock = new Stopwatch();
            //thread = new Thread(__run);
            clock.Start();
            //thread.Start();
            __run();
        }

        private void __run()
        {
            Qontext qontext = new Qontext(Qontext.globalContext);
            ConsoleKeyInfo keyInfo;
            draw();
            setCursor(0);
            do {
                while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter) {
                    if (keyInfo.Key == ConsoleKey.Backspace) {
                        if (input.Length > 0 && cx > 0) {
                            line.RemoveAt(cx - 1);
                            setCursor(cx - 1);
                        }
                    } else if (keyInfo.Key == ConsoleKey.Delete) {
                        if (input.Length > 0 && cx < line.Count) {
                            line.RemoveAt(cx);
                        }
                    } else if (keyInfo.Key == ConsoleKey.Tab) {
                        var match = qontext.names.Keys
                            .FirstOrDefault(item => item != input && item.StartsWith(input, true, CultureInfo.InvariantCulture));
                        if (string.IsNullOrEmpty(match))
                            continue;

                        write(match);
                    } else if (keyInfo.Key == ConsoleKey.UpArrow) {
                        buffer.Clear();
                        if (--historyIndex < 0) {
                            historyIndex = -1;
                            line.Clear();
                    } else {
                            line = history[historyIndex].ToCharArray().ToList();
                        }
                        setCursor(line.Count, 0);
                    } else if (keyInfo.Key == ConsoleKey.DownArrow) {
                        buffer.Clear();
                        if (++historyIndex >= history.Count) {
                            historyIndex = history.Count;
                            line.Clear(); 
                        } else {
                            line = history[historyIndex].ToCharArray().ToList();
                        }
                        setCursor(line.Count, 0);
                    } else if (keyInfo.Key == ConsoleKey.LeftArrow) {
                        setCursor(Math.Max(0, cx - 1));
                    } else if (keyInfo.Key == ConsoleKey.RightArrow) {
                        setCursor(Math.Min(input.Length, cx + 1));
                    } else {
                        line.Insert(Math.Min(cx, line.Count), keyInfo.KeyChar);
                        setCursor(cx + 1);
                    }
                    draw();
                }
                if ((keyInfo.Modifiers & ConsoleModifiers.Shift) != ConsoleModifiers.Shift) {
                    history.RemoveAll(_ => _ == input);
                    history.Add(input);
                    historyIndex = history.Count;
                    File.WriteAllText(HISTORY_FILE, string.Join<string>('\n', history.Select(_ => _.Replace("\n", " ").Trim()).ToArray<string>()));
                    write("\n");
                    //new Thread(() => runtime.execute(input)).Start();
                    runtime.execute(strip(input), qontext);
                    buffer.Clear();
                    line.Clear();
                } else {
                    buffer = buffer.Concat(line).ToList();
                    buffer.Add('\n');                        
                    line.Clear();
                    setCursor(0, 1);
                }
                draw();
                setCursor(0);
            } while (exitCode == 0);
        }

        private string strip(string input)
        {
            foreach (var c in colors.Values) {
                input = input.Replace(c.ToString(), "");
            }
            return input;
        }

        private string color()
        {
            var temp = ((char[])chars.ToArray().Clone()).ToList(); // perverted
            var level = log.loggingLevel;
            log.setLoggingLevel(Logger.Level.MUFFLE);
            try {
                int i = 0;
                var tokens = tokenResolver.resolve(new Stack<char>(chars.ToArray()));
                tokens.process((current, next, index, end) => {
                    var t = next();
                    if (mapping.contains(t.type)) {
                        var c = mapping[t.type];
                        temp.Insert((int)t.__pos + (i * 2), colors[c]);
                        temp.Insert((int)t.__end + 1 + (i++ * 2), colors[ConsoleColor.White]);
                    }
                });
            } catch (Exception e) {
                temp.Insert(0, colors[ConsoleColor.Red]);
                temp.Add(colors[ConsoleColor.White]);
            }
            log.setLoggingLevel(level);
            return new string(temp.ToArray());
        }

        private void draw()
        {
            var colored = color();
            var lines = colored.Split("\n");
            int y = 0;
            int x = cx;
            int h = lines.Length - 1;
            setCursor(-prefix.Length, -h);
            foreach (var line in lines) {
                setCursor(-prefix.Length, 0);
                write(prefix + new string(' ', Console.WindowWidth - prefix.Length));
                setCursor(0, 0);
                write(line);
                if (h > 0) setCursor(0, 1);
                h--;
            }
            setCursor(x, 0);
        }

        private void setCursor(int x = 0, int y = 0)
        {
           x = Math.Max(0, Math.Min(prefix.Length + x, Console.WindowWidth));
           Console.SetCursorPosition(x, Console.CursorTop + y);
        }

        // das alles in eine console klasse
        private void write(string input)
        {
            foreach (var c in input.ToCharArray()) {
                if (colors.Values.Contains(c)) {
                    Console.ForegroundColor = colors.findOne(_ => _ == c).Key;
                } else {
                    Console.Write(c);
                }
            }
        }
    }
}

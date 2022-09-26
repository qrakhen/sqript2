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

        private Qontext qontext;
        private Thread thread;
        private Stopwatch clock;
        private int historyIndex = 0;
        private int currentLine = 0;
        private List<char> line => lines[currentLine];
        private List<List<char>> lines = new List<List<char>>();
        private string lineStr => new string(lines[currentLine].ToArray());
        private string word => lineStr.Split(" ").Last();

        private int cx => Console.CursorLeft - prefix.Length;
        private int cy => Console.CursorTop;

        public List<string> history { get; private set; } = new List<string>();
        public int exitCode { get; private set; } = 0;

        public void run(Qontext qontext)
        {
            this.qontext = qontext;

            if (!File.Exists(HISTORY_FILE)) {
                File.Create(HISTORY_FILE);
            } else {
                history = File.ReadAllText(HISTORY_FILE).Split("\n").ToList();
            }
            historyIndex = history.Count;
            clock = new Stopwatch();
            clock.Start();
            __run(qontext);
        }

        private void __run(Qontext qontext)
        {
            log.cmd("Safe mode: Use shift + enter for a new line. Execute multiple lines using ctrl + enter.");
            reset();
            ConsoleKeyInfo keyInfo;
            draw();
            setCursor(0);
            do {
                while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter) {
                    if (keyInfo.Key == ConsoleKey.Backspace) {
                        if (lineStr.Length > 0 && cx > 0) {
                            line.RemoveAt(cx - 1);
                            setCursor(cx - 1);
                        }
                    } else if (keyInfo.Key == ConsoleKey.Delete) {
                        if (lineStr.Length > 0 && cx < line.Count) {
                            line.RemoveAt(cx);
                        }
                    } else if (keyInfo.Key == ConsoleKey.Tab) {
                        var match = qontext.names.Keys
                            .FirstOrDefault(item => item != lineStr && item.StartsWith(lineStr, true, CultureInfo.InvariantCulture));
                        if (string.IsNullOrEmpty(match))
                            continue;

                        write(match);
                    } else if (keyInfo.Key == ConsoleKey.UpArrow) {
                        moveLine(-1);
                    } else if (keyInfo.Key == ConsoleKey.DownArrow) {
                        moveLine(1);
                    } else if (keyInfo.Key == ConsoleKey.LeftArrow) {
                        setCursor(Math.Max(0, cx - 1));
                    } else if (keyInfo.Key == ConsoleKey.RightArrow) {
                        setCursor(Math.Min(lineStr.Length, cx + 1));
                    } else {
                        line.Insert(Math.Min(cx, line.Count), keyInfo.KeyChar);
                        setCursor(cx + 1);
                    }

                    /*if (word.Length > 2) {
                        var match = qontext.names.Keys
                            .FirstOrDefault(item => item != input && item.StartsWith(input, true, CultureInfo.InvariantCulture));
                        if (string.IsNullOrEmpty(match))
                            continue;

                        //line.Insert(Math.Min(cx, line.Count), keyInfo.KeyChar);
                    }*/

                    draw();
                }
                var shift = ((keyInfo.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift);
                if ((lines.Count == 1 && !shift) || ((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)) {
                    execute();
                } else if (lines.Count > 1 || shift) {
                    lines.Add(new List<char>());
                    moveLine(1);
                    draw();
                    setCursor(0);
                }
            } while (exitCode == 0);
        }

        private void moveLine(int delta)
        {
            if (currentLine + delta < 0 || currentLine + delta >= lines.Count) {
                moveHistory(delta);
                return;
            }

            currentLine += delta;
            setCursor(cx > line.Count ? line.Count : cx, delta);
        }

        private void moveHistory(int delta)
        {
            reset();
            historyIndex += delta;
            if (historyIndex < 0) {
                historyIndex = -1;
            } else if (historyIndex >= history.Count) {
                historyIndex = history.Count;
            } else {
                loadHistoryEntry(history[historyIndex]);
            }
            setCursor(line.Count, 0);
        }

        private void reset(bool doDraw = true)
        {
            if (doDraw) drawReset();
            lines.Clear();
            lines.Add(new List<char>());
            currentLine = 0;
            draw();
        }

        private void execute()
        {
            storeHistory();
            write("\n");
            runtime.execute(strip(concatLines()), qontext);
            reset(false);
            setCursor(0);
        }

        private string concatLines()
        {
            return string.Join("\n", lines.Select(_ => new string(_.ToArray())));
        }

        private void loadHistoryEntry(string entry)
        {
            reset();
            var lines = entry.Split("%N%", StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
                return;
            this.lines = new List<List<char>>();
            int index = 0;
            foreach (var line in lines) {
                if (index++ > 0) {
                    setCursor(cx, 1);
                    currentLine++;
                }
                this.lines.Add(line.ToCharArray().ToList());
                draw();
            }
        }

        private void storeHistory()
        {
            var str = concatLines().Replace("\n", "%N%");
            history.RemoveAll(_ => _ == str);
            while (history.Count > 100) {
                history.RemoveAt(0);
            } 
            history.Add(str);
            historyIndex = history.Count;
            File.WriteAllText(HISTORY_FILE, string.Join<string>('\n', history.Select(_ => _.Trim()).ToArray<string>()));
        }

        private string strip(string input)
        {
            foreach (var c in colors.Values) {
                input = input.Replace(c.ToString(), "");
            }
            return input;
        }

        private string color(List<char> chars)
        {
            var temp = ((char[])chars.ToArray().Clone()).ToList(); // perverted
            var level = log.loggingLevel;
            log.setLoggingLevel(Logger.Level.MUFFLE);
            try {
                int i = 0;
                var tokens = tokenResolver.resolve(new Stack<char>(chars.ToArray()), qontext);
                tokens.process((current, next, index, end) => {
                    var t = next();
                    if (mapping.contains(t.type)) {
                        var c = mapping[t.type];
                        if (index > 1 && tokens.peek(-2).raw == ":") {
                            c = ConsoleColor.DarkCyan;
                        }
                        temp.Insert((int)t.__pos + (i * 2), colors[c]);
                        temp.Insert((int)t.__end + 1 + (i++ * 2), colors[ConsoleColor.White]);
                    }
                });
            } catch (Exception e) {
                if (e is SqrError) {
                    var d = (e as SqrError).data;
                    if (d is Token) {
                        temp.Insert(Math.Max(0, (int)(d as Token).__pos), colors[ConsoleColor.Red]);
                        temp.Insert(Math.Min((int)(d as Token).__end + 1, temp.Count), colors[ConsoleColor.White]);
                        log.setLoggingLevel(level);
                        return new string(temp.ToArray());

                    }
                }

                temp.Insert(0, colors[ConsoleColor.Red]);
                temp.Add(colors[ConsoleColor.White]);
            }
            log.setLoggingLevel(level);
            return new string(temp.ToArray());
        }

        private void draw()
        {
            Console.CursorVisible = false;
            var colored = color(line);
            var str = new string(colored);          
            int x = cx;
            setCursor(-prefix.Length, 0);
            write(prefix + new string(' ', Console.WindowWidth - prefix.Length - 1));
            setCursor(0, 0);
            write(str);
            setCursor(x, 0);
            Console.CursorVisible = true;
        }

        private void drawReset()
        {
            Console.CursorVisible = false;
            int y = lines.Count - 1;
            int h = y;
            setCursor(-prefix.Length, -currentLine);
            foreach (var line in lines) {
                setCursor(-prefix.Length, 0);
                write(new string(' ', Console.WindowWidth - 1));
                setCursor(0, 0);
                if (h > 0) setCursor(0, 1);
                h--;
            }
            setCursor(0, -(y));
            Console.CursorVisible = true;
        }

        private void setCursor(int x = 0, int y = 0)
        {
            x = Math.Max(0, Math.Min(prefix.Length + x, Console.WindowWidth - 1));
            if (cy + y <= 0)
                y = 0;
            Console.SetCursorPosition(x, Console.CursorTop + y);
        }

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

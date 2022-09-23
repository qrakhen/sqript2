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

namespace Qrakhen.Sqr.Core 
{
    [Injectable]
    public class Runtime 
    {
        public static readonly Qonfig qonfig = new Qonfig();

        private readonly Logger log;
        private readonly UCI userControlInterface;
        private readonly TokenResolver tokenResolver;
        private readonly OperationResolver operationResolver;
        private readonly ValueResolver valueResolver;

        public bool alive { get; private set; } = true;

        public void init()
        {
            log.setLoggingLevel(Logger.Level.VERBOSE);
        }

        public Qontext run(string content = null, bool __DEV_DEBUG = false) 
        {           
            if (content != null) {
                log.setLoggingLevel(Logger.Level.CRITICAL);
                if (content.StartsWith("!!")) {
                    content = content[2..];
                    __DEV_DEBUG = true;
                } else if (content.EndsWith("!!")) {
                    content = content[0..^2];
                    __DEV_DEBUG = true;
                }

                Module module = null;
                var moduleKeyword = Keyword.get(Keyword.Type.MODULE).symbol;
                if (content.StartsWith(moduleKeyword)) {
                    string name = content.Substring(moduleKeyword.Length, content.IndexOf(";")).Trim();
                    module = new Module(name, null, Qontext.globalContext);
                }
                Qontext qontext = module == null ? Qontext.globalContext : module.qontext;

                if (__DEV_DEBUG)
                    log.setLoggingLevel(Logger.Level.SPAM);

                execute(content, qontext);
                return qontext;
            } else {
                log.write(Properties.strings.ASCII_Logo, ConsoleColor.DarkGray, prefix: "    ");
                log.success(Properties.strings.Message_Welcome);
                if (qonfig.useExtendedConsole) {
                    userControlInterface.run();
                } else {
                    Qontext consoleQontext = new Qontext();
                    do {
                        execute(Console.ReadLine(), consoleQontext);
                    } while (true);
                }
                return null;
            }
        }

        internal void registerGlobalFunqtions(Qontext qontext) 
        {
            if (qontext.names["cout"] != null)
                return;

            qontext.register(
                "cout",
                new Qallable(new InternalFunqtion((p, q, s) => { log.success(p[0].raw); return Value.Void; })));
            qontext.register(
                "rand",
                new Qallable(new InternalFunqtion((p, q, s) => { return new Number(new Random().NextDouble()); })));
            qontext.register(
                "log",
                new Qallable(new InternalFunqtion((p, q, s) => { log.setLoggingLevel((Logger.Level)int.Parse(p[0].raw.ToString())); return Value.Void; })));
            qontext.register(
                "import",
                new Qallable(new InternalFunqtion((p, q, s) => {
                    qontext.import(run(File.ReadAllText(p[0] as String)));
                    return Value.Void;
                })));
            qontext.register(
               "export",
               new Qallable(new InternalFunqtion((p, q, s) => {
                   qontext.export(p[0].raw as string, p[0]);
                   return Value.Void;
               })));
        }        

        public void execute(string input, Qontext qontext)
        {
            //qontext = qontext ?? Qontext.globalContext;
            registerGlobalFunqtions(qontext);
            try {                
                if (input.StartsWith("/")) {
                    commands(input.Substring(1), qontext);
                    return;
                } else if (string.IsNullOrEmpty(input)) {
                    log.info("yes i too am kinda lazy today, it's fine.");
                    return;
                }

                var t = new Stopwatch();
                t.Restart();
                long _ms = 0, _t = 0;
                var tokenStack = tokenResolver.resolve(new Core.Stack<char>(applyAliases(input).ToCharArray()));
                while (!tokenStack.done) {
                    var operation = operationResolver.resolveOne(tokenStack, qontext);
                    var result = operation.execute(qontext);
                    if (result != null) {
                        if (log.loggingLevel > Logger.Level.INFO)
                            log.success(result.toDebugString());
                        else
                            log.success(result.ToString());
                    }
                    log.verbose("operation time " + (t.ElapsedMilliseconds - _ms) + "ms, " + (t.ElapsedTicks - _t) + " ticks");
                    _ms = t.ElapsedMilliseconds;
                    _t = t.ElapsedTicks;
                }
                log.info("execution time " + (t.ElapsedMilliseconds) + "ms, " + (t.ElapsedTicks) + " ticks");
            } catch (SqrError e) {
                log.error(log.loggingLevel > Logger.Level.INFO ? e : (object)e.Message);
                log.warn("Sqr stacktrace:\n" + string.Join("\n", SqrError.stackTrace.ToArray()));
                if (e.data != null && log.loggingLevel >= Logger.Level.DEBUG)
                    log.warn(json(e.data));
            } /* catch (Exception e) {
                    log.error("### system exceptions need to be completely eradicated ###");
                    log.error(e);
                    log.error("### system exceptions need to be completely eradicated ###");
                    throw e;
                }*/
        }

        private void commands(string input, Qontext qontext) 
        {
            var args = input.Split(" ");
            if (input == "q") {
                log.cmd(json(qontext));
                return;
            } else if (input.StartsWith("p")) {
                log.cmd(json(
                    valueResolver.resolve(
                        new Stack<Token>(
                            new Token[] {
                                Token.create(args[1], Token.Type.Identifier)
                            }), Qontext.globalContext)));
                return;
            } else if (input.StartsWith("alias")) {
                if (args.Length == 1) {
                    log.cmd(aliases);
                } else if (args.Length == 2) {
                    aliases.Remove(args[1]);
                    log.cmd("removed alias for " + args[1]);
                } else if (args.Length == 3) {
                    aliases[args[1]] = args[2];
                    log.cmd("added/updated alias " + args[1] + " > " + args[2]);
                } else {
                    log.error("usage: /alias [from] [to], example: /alias *~ var");
                }
            } else if (input.StartsWith("help")) {
                log.warn("the help DLC is available on steam for $39.95");
            } else if (input.StartsWith("log")) {
                if (args.Length == 1) {
                    log.cmd("current level: " + (int) log.loggingLevel);
                    foreach (var i in Enum.GetValues(typeof(Logger.Level))) {
                        log.cmd((int) i + ": " + (Logger.Level) i);
                    }
                } else if (args.Length == 2) {
                    log.setLoggingLevel((Logger.Level)int.Parse(args[1]));
                    log.cmd("set logging level to " + log.loggingLevel);
                }
            } else if (input == "t") {
                var t = File.ReadAllText("tests.sqr");
                execute(t, qontext);
            } else if (input == "c") {
                qontext.names.clear();
                log.cmd("cleared global qontext");
            }
        }

        private string applyAliases(string value) 
        {
            log.verbose("applying aliases:");
            foreach (var alias in aliases) {
                if (alias.Key.Contains("$")) {
                    log.verbose("  + " + alias.Key + " > " + alias.Value);
                    var pattern = Regex.Escape(alias.Key).Replace("\\$", "(.+?)");
                    var m = Regex.Matches(value, pattern);
                    for (var i = 0; i < m.Count; i++) {
                        value = value.Replace(m[i].Groups[0].Value, alias.Value.Replace("$", m[i].Groups[1].Value));
                    }
                } else {
                    log.verbose("  + " + alias.Key + " > " + alias.Value);
                    value = value.Replace(alias.Key, " " + alias.Value + " ");
                }
            }
            return value;
        }

        private string json(object any) 
        {
            return JsonConvert.SerializeObject(
                any,
                Formatting.Indented,
                new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    MaxDepth = 3                  
                }
            );
        }

        static readonly private Dictionary<string, string> aliases = new Dictionary<string, string>() 
        {
            { "*~", "var" },
            { "*&", "ref" },
            { "*$~", "@$" },
            { "*$&", "@$&" }
        };
    }
}

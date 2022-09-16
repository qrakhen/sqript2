using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class Runtime
    {
        private readonly Logger log;
        private readonly TokenDigester tokenDigester;
        private readonly OperationDigester operationDigester;

        public bool alive { get; private set; } = true;

        public void run()
        {
            log.setLoggingLevel(Logger.Level.VERBOSE);
            log.success("welcome to Sqript2.0, or simply sqr. Enjoy thyself.");
            do {
                try {

                    //var breaktest = ": a ! x ooo p ? ! 'astr\\'ing' asd 5 / 3 * 10 - 43 asdasdasasdas true*** falseaaaa true ':asd:dAD'adAS:Asd:[]3414:As ! 231 111 12.321 1,1,1,";
                    //new List<Token>(tokenDigester.digest(new Core.Stack<char>(breaktest.ToCharArray()))).ForEach(Console.WriteLine);

                    var op = "*~ a <~ 5;"; // 2 - 3 + 3 * 3 / 5 + test:von:mama:her";

                    Console.Write(" <: ");
                    execute(Console.ReadLine());

                } catch (SqrError e) {
                    log.error(log.loggingLevel > Logger.Level.INFO ? e : (object)e.Message);
                    log.warn("are you by any chance stupid?");
                } catch (Exception e) {
                    log.error("### system exceptions need to be completely eradicated ###");
                    log.error(e);
                    log.error("### system exceptions need to be completely eradicated ###");
                    throw e;
                }
            } while (alive);
        }

        private void execute(string input)
        {
            if (input.StartsWith("/")) {
                commands(input.Substring(1));
                return;
            } else if (string.IsNullOrEmpty(input)) {
                log.info("yes i too am kinda lazy today, it's fine.");
                return;
            }

            var t = tokenDigester.digest(new Core.Stack<char>(applyAliases(input).ToCharArray()));
            log.spam("token list: ");
            new List<Token>(t).ForEach(_ => log.spam(_));
            var r = operationDigester.digest(new Stack<Token>(t), Qontext.globalContext);
            log.success(r.execute());
        }

        private void commands(string input)
        {
            log.spam("command " + input);
            if (input == "q") {
                log.cmd(json(Qontext.globalContext));
                return;
            } else if (input.StartsWith("alias")) {
                var args = input.Split(" ");
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
                var args = input.Split(" ");
                if (args.Length == 1) {
                    log.cmd("current level: " + (int)log.loggingLevel);
                    foreach (var i in Enum.GetValues(typeof(Logger.Level))) {
                        log.cmd((int)i + ": " + (Logger.Level)i);
                    }
                } else if (args.Length == 2) {
                    log.setLoggingLevel((Logger.Level)int.Parse(args[1]));
                    log.cmd("set logging level to " + log.loggingLevel);
                }
            }
        }

        private string applyAliases(string value)
        {
            log.spam("applying aliases");
            foreach (var alias in aliases) {
                log.spam("applying " + alias.Key + " > " + alias.Value);
                value = value.Replace(alias.Key, " " + alias.Value + " ");
            }
            return value;
        }

        private string json(object any)
        {
            return JsonConvert.SerializeObject(
                any,
                Formatting.Indented,
                new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            );
        }

        static readonly private Dictionary<string, string> aliases = new Dictionary<string, string>() {
            { "*~", "var" },
            { "*&", "ref" }
        };
    }
}

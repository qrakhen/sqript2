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
            do {
                try {

                    //var breaktest = ": a ! x ooo p ? ! 'astr\\'ing' asd 5 / 3 * 10 - 43 asdasdasasdas true*** falseaaaa true ':asd:dAD'adAS:Asd:[]3414:As ! 231 111 12.321 1,1,1,";
                    //new List<Token>(tokenDigester.digest(new Core.Stack<char>(breaktest.ToCharArray()))).ForEach(Console.WriteLine);

                    var op = "*~ a <~ 5;"; // 2 - 3 + 3 * 3 / 5 + test:von:mama:her";

                    execute(Console.ReadLine());

                } catch (SqrError e) {
                    log.error(e);
                    //throw e;
                } catch (Exception e) {
                    log.error("### system exceptions need to be completely eradicated ###");
                    log.error(e);
                    log.error("### system exceptions need to be completely eradicated ###");
                    //throw e;
                }
            } while (alive);
        }

        private void execute(string input)
        {
            var t = tokenDigester.digest(new Core.Stack<char>(applyAliases(input).ToCharArray()));
            new List<Token>(t).ForEach(Console.WriteLine);
            var r = operationDigester.digest(new Stack<Token>(t), Qontext.globalContext);
            log.debug(r.execute());
        }

        private string applyAliases(string value)
        {
            foreach (var alias in aliases)
                value = value.Replace(alias.Key, alias.Value + " ");
            return value;
        }

        static readonly private Dictionary<string, string> aliases = new Dictionary<string, string>() {
            { "*~", "var" },
            { "*&", "ref" }
        };
    }
}

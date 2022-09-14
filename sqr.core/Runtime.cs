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

        public bool alive { get; private set; }

        public void run()
        {
            do {
                //try {

                    //var breaktest = ": a ! x ooo p ? ! 'astr\\'ing' asd 5 / 3 * 10 - 43 asdasdasasdas true*** falseaaaa true ':asd:dAD'adAS:Asd:[]3414:As ! 231 111 12.321 1,1,1,";
                    //new List<Token>(tokenDigester.digest(new Core.Stack<char>(breaktest.ToCharArray()))).ForEach(Console.WriteLine);

                var op = "5 + 7 / 12 - 3 * 9 + 1";
                var t = tokenDigester.digest(new Core.Stack<char>(op.ToCharArray()));
                new List<Token>(t).ForEach(Console.WriteLine);
                operationDigester.digest(new Stack<Token>(t), Qontext.globalContext);
                Console.ReadLine();

                /*} catch (SqrError e) {
                    log.error(e);
                    throw e;
                } catch (Exception e) {
                    log.error("### system exceptions need to be completely eradicated ###");
                    log.error(e);
                    log.error("### system exceptions need to be completely eradicated ###");
                    throw e;
                }*/
            } while (alive);
        }
    }
}

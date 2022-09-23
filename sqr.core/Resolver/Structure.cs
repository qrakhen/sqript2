using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    internal class StructureResolver : Resolver<Stack<Token>, Stack<Token>>
    {
        public Stack<Token> resolve(Stack<Token> input, Qontext qontext)
        {
            log.verbose("in " + GetType().Name);
            int level = 0;
            List<Token> buffer = new List<Token>();
            var t = input.peek();
            if (!t.isType(Token.Type.Structure))
                throw new SqrError("not a structure: " + t, t);

            var structure = input.digest().get<Structure>();
            log.spam("starting to read structure beginning from " + structure.open);
            do {
                t = input.digest();
                log.spam(t);
                if (t.raw == structure.open) {
                    level++;
                    log.spam("incremented level: " + level);
                } else if (t.raw == structure.close) {
                    if (level == 0) {
                        log.spam("done");
                        break;
                    } else {
                        level--;
                        log.spam("decremented level: " + level);
                    }
                }
                buffer.Add(t);
            } while (!input.done);
            log.spam("digested " + buffer.Count + " items");
            return new Stack<Token>(buffer.ToArray());
        }

        public Stack<Token> resolveUntil(Stack<Token> input, Qontext qontext, string until, bool includeLast = false)
            => resolveUntil(input, qontext, new string[] { until }, includeLast);


        /// <summary>
        /// leveled digest until symbol, not including symbol
        /// </summary>
        /// <param name="input"></param>
        /// <param name="qontext"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        public Stack<Token> resolveUntil(Stack<Token> input, Qontext qontext, string[] until, bool includeLast = false)
        {
            log.verbose("in " + GetType().Name);
            int level = 0;
            List<Token> buffer = new List<Token>();
            var t = input.peek();
            log.spam("starting to read subset of structure, starting from " + t + " until " + until);
            do {
                t = input.digest();
                log.spam(t);
                if (until.Contains(t.raw) && level == 0) {
                    if (includeLast) buffer.Add(t);
                    break;
                }

                if (Structure.openers.Contains(t.raw)) {
                    level++;
                    log.spam("incremented level: " + level);
                } else if (Structure.closers.Contains(t.raw)) {
                    if (level == 0) {
                        log.spam("done prematurely, some structure ended before " + until + " was reached");
                        break;
                    } else {
                        level--;
                        log.spam("decremented level: " + level);
                    }
                }
                buffer.Add(t);
            } while (!input.done);
            log.spam("digested " + buffer.Count + " items");
            return new Stack<Token>(buffer.ToArray());
        }
    }
}

using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class StructureResolver : Resolver<Stack<Token>, Stack<Token>>
    {
        public override Stack<Token> resolve(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);
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

        /// <summary>
        /// leveled digest until symbol, not including symbol
        /// </summary>
        /// <param name="input"></param>
        /// <param name="qontext"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        public override Stack<Token> resolveUntil(Stack<Token> input, Qontext qontext, string until)
        {
            log.spam("in " + GetType().Name);
            int level = 0;
            List<Token> buffer = new List<Token>();
            var t = input.peek();
            log.spam("starting to read subset of structure, starting from " + t + " until " + until);
            do {
                t = input.digest();
                log.spam(t);
                if (t.raw == until)
                    break;

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

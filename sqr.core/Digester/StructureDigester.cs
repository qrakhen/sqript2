using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class StructureDigester : Digester<Stack<Token>, Token[]>
    {
        public override Token[] digest(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);
            int level = 0;
            List<Token> buffer = new List<Token>();
            var t = input.peek();
            if (!t.isType(Token.Type.Structure))
                throw new SqrError("nop");

            var structure = input.digest().get<Structure>();
            do {
                t = input.digest();
                buffer.Add(t);
                if (t.raw == structure.open) {
                    level++;
                } else if (t.raw == structure.close) {
                    if (level == 0)
                        break;
                    else
                        level--;
                }                
            } while (!input.done);

            return buffer.ToArray();
        }

        /// <summary>
        /// leveled digest until symbol, not including symbol
        /// </summary>
        /// <param name="input"></param>
        /// <param name="qontext"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        public override Token[] digestUntil(Stack<Token> input, Qontext qontext, string until)
        {
            log.spam("in " + GetType().Name);
            int level = 0;
            List<Token> buffer = new List<Token>();
            var t = input.peek();
            do {
                t = input.digest();
                if (t.raw == until)
                    break;

                buffer.Add(t);
                if (Structure.openers.Contains(t.raw)) {
                    level++;
                } else if (Structure.closers.Contains(t.raw)) {
                    if (level == 0)
                        break;
                    else
                        level--;
                }
            } while (!input.done);

            return buffer.ToArray();
        }
    }
}

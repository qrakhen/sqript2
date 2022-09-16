using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class StructureDigester : Digester<Stack<Token>, Value>
    {
        public Value digest(Stack<Token> input, Qontext qontext)
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
                if (t.get<string>() == structure.open) {
                    level++;
                } else if (t.get<string>() == structure.close) {
                    if (level == 0)
                        break;
                    else
                        level--;
                }                
            } while (!input.done);

            var stack = new Stack<Token>(buffer.ToArray());

            return null;
        }
    }
}

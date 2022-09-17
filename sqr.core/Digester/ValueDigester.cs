using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Qrakhen.Sqr.Core
{  
    [Injectable]
    public class ValueDigester : Digester<Stack<Token>, Value>
    {
        private readonly Logger log;
        private readonly StructureDigester structureDigester;

        public override Value digest(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);
            Token t = input.peek();

            if (!t.isType(Token.Type.Value))
                throw new SqrError("token is not a value: " + t);

            if (!t.isType(Token.Type.Identifier)) {
                log.spam("returning " + t.makeValue());
                return input.digest().makeValue();
            }

            List<string> name = new List<string>();
            input.process(() => input.peek().isType(Token.Type.Identifier), (current, index, abort) => {
                name.Add(input.digest().value.ToString());
                if (current() != null && current().isType(Token.Type.Accessor)) {
                    input.digest();
                }
            });
            log.debug("detected name " + string.Join(":", name));
            var value = qontext.resolveName(name.ToArray());
            log.debug("value to that name: " + value);
            return value;
        }
    }
}

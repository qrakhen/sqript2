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
            if (t.isType(Token.Type.Structure)) // bei [] array und bei () group => value durch execute (node)
                return null;// structureDigester.digest(input, qontext);

            if (!t.isType(Token.Type.Value))
                throw new SqrError("token is not a value: " + t);

            if (!t.isType(Token.Type.Identifier))
                return input.digest().makeValue();

            List<string> name = new List<string>();
            input.process(() => input.peek().isType(Token.Type.Identifier), (v) => {
                name.Add(input.digest().value.ToString());
                if (input.peek() != null && input.peek().isType(Token.Type.Accessor)) {
                    input.digest();
                }
            });
            log.debug("detected name " + string.Join(":", name));
            var value = qontext.resolveName(name.ToArray());
            return value;
        }
    }
}

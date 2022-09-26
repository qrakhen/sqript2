using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using static Qrakhen.Sqr.Core.Operation;

namespace Qrakhen.Sqr.Core
{  
    [Injectable]
    internal class ValueResolver : Resolver<Stack<Token>, Value>
    {
        private readonly Logger log;
        private readonly StructureResolver structureResolver;
        private readonly QollectionResolver qollectionResolver;

        public Value resolve(Stack<Token> input, Qontext qontext)
        {
            log.verbose("in " + GetType().Name);
            Token t = input.peek();

            if (!t.isType(Token.Type.Value))
                throw new SqrError("token is not a value: " + t, t);

            Value value = null;

            if (!t.hasType(Token.Type.Identifier)) {
                log.spam("got primitive " + t);
                if (value == null)
                    value = input.digest().makeValue();
            } else {
                log.spam("got identifier " + t.raw);
                if (value == null) // root identifier of possible member chain
                    value = qontext.resolveName(input.digest().get<string>());
            }

            return value;
        }
    }
}

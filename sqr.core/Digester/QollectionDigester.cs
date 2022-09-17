using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class QollectionDigester : Digester<Stack<Token>, Qollection>
    {
        private readonly ValueDigester valueDigester;

        public override Qollection digest(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);
            input.process((current, index) => {
                var t = current();
                if (index == 0) {
                    if (t.isType(Token.Type.Structure) && t.get<Structure>().type != Structure.Type.QOLLECTION) {
                        input.digest();
                    } else {
                        throw new SqrError("not a valid qollection structure start!" + t);
                    }
                } else {
                    t = current();
                    if (t.isType(Token.Type.Value)) {
                        var v = valueDigester.digest(input, qontext);
                        log.spam("found value for qollection: " + v);
                        t = current();
                        if (t.type != Token.Type.Structure && t.get<Structure>().extra != t.raw)
                            throw new SqrError("expected " + t.get<Structure>().extra + ", got " + t.raw);

                    }
                }
            });
            return null;
        }
    }
}

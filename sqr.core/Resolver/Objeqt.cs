using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class ObjeqtResolver : Resolver<Stack<Token>, Objeqt>
    {
        private readonly ValueResolver valueResolver;
        private readonly StructureResolver structureResolver;
        private readonly OperationResolver operationResolver;

        public override Objeqt resolve(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);
            var objeqt = new Objeqt();
            var separator = Structure.get(Structure.Type.BODY).separator;
            input.process((current, take, index, abort) => 
            {
                var sub = structureResolver.resolveUntil(input, qontext, separator);
                log.spam("digested sub (until " + separator + "): " + string.Join(' ', sub.items.Select(_ => _.ToString())));
                var name = sub.digest();
                if (!name.isType(Token.Type.Identifier))
                    throw new SqrError("expected name literal for objeqt key, but got " + name, name);



                var op = operationResolver.resolve(sub, qontext);
                var r = op.execute();
                log.spam("adding result: " + r);
                qollection.add(r.getValue() as Value);
            });
            return objeqt;
        }
    }
}

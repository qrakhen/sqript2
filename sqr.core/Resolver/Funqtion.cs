using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class FunqtionResolver : Resolver<Stack<Token>, Funqtion>
    {
        private readonly ValueResolver valueResolver;
        private readonly StructureResolver structureResolver;
        private readonly OperationResolver operationResolver;

        /// <summary>
        /// expects a trimmed token list (no encloding structures)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="qontext"></param>
        /// <returns></returns>
        public Funqtion resolve(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);
            var qollection = new Qollection();
            var separator = Structure.get(Structure.Type.QOLLECTION).separator;
            input.process((current, take, index, abort) => {
                // outsourcing the entire level/structure logic, should do that more often
                var sub = structureResolver.resolveUntil(input, qontext, separator);
                log.spam("digested sub (until " + separator + "): " + string.Join(' ', sub.items.Select(_ => _.ToString())));
                var op = operationResolver.resolve(sub, qontext);
                var r = op.execute();
                log.spam("adding result: " + r);
                qollection.add(r.obj as Value);
            });
            return qollection;
        }
    }
}

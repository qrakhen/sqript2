using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class QollectionDigester : Digester<Stack<Token>, Qollection>
    {
        private readonly ValueDigester valueDigester;
        private readonly StructureDigester structureDigester;
        private readonly OperationDigester operationDigester;

        /// <summary>
        /// expects a trimmed token list (no encloding structures)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="qontext"></param>
        /// <returns></returns>
        public override Qollection digest(Stack<Token> input, Qontext qontext, object separator)
        {
            log.spam("in " + GetType().Name);
            var qollection = new Qollection();
            input.process((current, index, abort) => {
                // outsourcing the entire level/structure logic, should do that more often
                var sub = structureDigester.digestUntil(input, qontext, (string)separator);
                log.spam("digested sub (until " + separator + "): " + string.Join(' ', sub.items.Select(_ => _.ToString())));
                var op = operationDigester.digest(sub, qontext);
                var r = op.execute();
                log.spam("adding result: " + r);
                qollection.add(r.getValue() as Value);
            });
            return qollection;
        }
    }
}

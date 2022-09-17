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

        public override Qollection digest(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);
            Structure structure = null;
            var list = new List();
            input.process(() => (input.peek().raw == structure?.close), (current, index, abort) => {
                var t = current();
                if (index == 0) {
                    if (t.isType(Token.Type.Structure) && t.get<Structure>().type != Structure.Type.QOLLECTION) {
                        input.digest();
                        structure = t.get<Structure>();
                    } else {
                        throw new SqrError("not a valid qollection structure start!" + t);
                    }
                } else {
                    if (current().raw == structure.close) {
                        log.spam("done with qollection");
                        input.digest();
                        abort();
                    } else {
                        // outsourcing the entire level/structure logic, should do that more often
                        var sub = structureDigester.digestUntil(input, qontext, structure.separator);
                        log.spam("digested sub (until " + structure.separator + "): " + string.Join(' ', sub.ToList().Select(_ => _.ToString())));
                        var op = operationDigester.digest(new Stack<Token>(sub), qontext);
                        var r = op.execute();
                        log.spam("adding result: " + r);
                        list.add(r);
                    }
                }
            });
            return null;
        }
    }
}

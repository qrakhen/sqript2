using Qrakhen.SqrDI;
using System.Linq;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    internal class ObjeqtResolver : Resolver<Stack<Token>, Objeqt>
    {
        private readonly ValueResolver valueResolver;
        private readonly StructureResolver structureResolver;
        private readonly OperationResolver operationResolver;

        public Objeqt resolve(Stack<Token> input, Qontext qontext)
        {
            log.verbose("in " + GetType().Name);
            var objeqt = new Objeqt();
            var separator = Structure.get(Structure.Type.BODY).separator;
            input.process((current, take, index, abort) => 
            {
                var sub = structureResolver.resolveUntil(input, qontext, separator);
                log.spam("digested sub (until " + separator + "): " + string.Join(' ', sub.items.Select(_ => _.ToString())));
                var name = sub.digest();
                if (!name.isType(Token.Type.Identifier))
                    throw new SqrError("expected name literal for objeqt key, but got " + name, name);

                var op = sub.digest().get<Operator>();
                if (!op.isType(Operator.Type.ASSIGN | Operator.Type.ASSIGN_REF))
                    throw new SqrError("assign operator expected " + op, op);

                var value = operationResolver.resolveOne(sub, qontext).execute(qontext);
                var property = new Variable(value);
                property.set(value, op.isType(Operator.Type.ASSIGN_REF));
                objeqt.properties[name.raw] = property;

                log.spam("adding " + name + " as " + value);
            });
            return objeqt;
        }
    }
}

using Qrakhen.Dependor;
using System.Collections.Generic;
using System.Linq;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class QlassResolver : Resolver<Stack<Token>, Objeqt>
    {
        private readonly ValueResolver valueResolver;
        private readonly StructureResolver structureResolver;
        private readonly OperationResolver operationResolver;

        public Objeqt resolve(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);

            var methods = new List<Type.Method>();
            var fields = new List<Type.Field>();

            return null; ;
        }
    }
}

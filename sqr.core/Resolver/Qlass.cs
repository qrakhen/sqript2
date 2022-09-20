using Qrakhen.Dependor;
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

            return null; ;
        }
    }
}

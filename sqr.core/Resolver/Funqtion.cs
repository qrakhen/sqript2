using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    internal class FunqtionResolver : Resolver<Stack<Token>, Funqtion>
    {
        private readonly QollectionResolver qollectionResolver;
        private readonly StructureResolver structureResolver;
        private readonly OperationResolver operationResolver;
        private readonly DeclarationResolver declarationResolver;

        /// <summary>
        /// expects a trimmed token list (no encloding structures)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="qontext"></param>
        /// <returns></returns>
        public Funqtion resolve(Stack<Token> input, Qontext qontext, IDeclareInfo info = new IDeclareInfo())
        {
            log.debug("in " + GetType().Name);
            var bodyStructure = Structure.get(Structure.Type.BODY);
            var header = resolveHeader(structureResolver.resolveUntil(input, qontext, bodyStructure.open), qontext);
            input.move(-1);
            var body = new Body(structureResolver.resolve(input, qontext).items);
            return new Funqtion(body, header.ToArray(), info.type);
        }

        private List<IDeclareInfo> resolveHeader(Stack<Token> stack, Qontext qontext)
        {
            //@todo hier auch IDeclareinfo verwenden ffs
            var headerStructure = Structure.get(Structure.Type.GROUP);
            var bodyStructure = Structure.get(Structure.Type.BODY);
            var parameters = new List<IDeclareInfo>();

            stack.process((current, take, index, abort) => {
                var sub = structureResolver.resolveUntil(stack, qontext, new string[] { headerStructure.separator, bodyStructure.open });
                var info = declarationResolver.resolve(sub, qontext, true);
                parameters.Add(info);               
            });

            return parameters;
        }
    }
}

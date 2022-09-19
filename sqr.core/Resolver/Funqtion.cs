using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class FunqtionResolver : Resolver<Stack<Token>, Funqtion>
    {
        private readonly QollectionResolver qollectionResolver;
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
            var bodyStructure = Structure.get(Structure.Type.BODY);
            var header = resolveHeader(structureResolver.resolveUntil(input, qontext, bodyStructure.open));
            input.setCursor(input.index - 1); // not nice. gotta find a way to make this consistent and tidy. (geht um (a b c { }) das { wird mitgegessen bei readStructure
            var body = new Body(structureResolver.resolve(input, qontext).items);
            return new Funqtion(body, header.ToArray(), NativeType.None);
        }

        private List<Funqtion.DeclaredParam> resolveHeader(Stack<Token> stack)
        {
            var headerStructure = Structure.get(Structure.Type.GROUP);
            var parameters = new List<Funqtion.DeclaredParam>();

            stack.process((current, take, index, abort) => {
                var t = take();
                if (!t.isType(Token.Type.Identifier))
                    throw new SqrError("expected identifier at funqtion header declaration, got " + t + " instead", t);

                var name = t.raw;
                var optional = false;
                var type = NativeType.None;

                if (!stack.done && current().raw == "?") {
                    optional = true;
                    take();
                }

                if (!stack.done && current().raw == headerStructure.separator) {
                    take();
                }

                parameters.Add(new Funqtion.DeclaredParam() {
                    name = name,
                    optional = optional,
                    type = type
                });
            });

            return parameters;
        }
    }
}

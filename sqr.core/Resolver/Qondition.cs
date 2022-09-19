using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class QonditionResolver : Resolver<Stack<Token>, Qondition>
    {
        private readonly ValueResolver valueResolver;
        private readonly StructureResolver structureResolver;
        private readonly OperationResolver operationResolver;

        public IfQondition resolveIfElse(Stack<Token> input, Qontext qontext)
        {
            var qondition = new IfQondition(resolve(input, qontext));
            if (!input.done && input.peek().raw == Keyword.get(Keyword.Type.QONDITION_ELSE).symbol) {
                qondition.chainElseIf(resolveIfElse(input, qontext));
            }
            return qondition;
        }

        public Qondition resolve(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);

            var type = input.digest().get<Keyword>();
            log.spam("resolving " + type.symbol + " condition");

            if (type.type == Keyword.Type.QONDITION_ELSE) {
                if (input.peek().isType(Token.Type.Keyword) && input.peek().get<Keyword>().type == Keyword.Type.QONDITION_IF) {
                    type = input.digest().get<Keyword>();
                }
            }

            Operation condition = null;
            bool hasCondition = input.peek().raw == Structure.get(Structure.Type.GROUP).open;
            if (!hasCondition && type.type == Keyword.Type.QONDITION_IF)
                throw new SqrError("condition expected. empty conditions are only valid for else { } groups.", input.peek());

            else if (hasCondition) {
                condition = operationResolver.resolveOne(
                    structureResolver.resolve(input, qontext), qontext);
            }

            if (input.peek().raw != Structure.get(Structure.Type.BODY).open)
                throw new SqrError("expected { after if, got " + input.peek() + " instead", input.peek());

            var body = new Body(structureResolver.resolve(input, qontext).items);

            return new Qondition(condition, body, qontext);
        }
    }
}

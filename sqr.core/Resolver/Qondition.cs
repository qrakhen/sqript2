using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    internal class QonditionResolver : Resolver<Stack<Token>, Qondition>
    {
        private readonly ValueResolver valueResolver;
        private readonly StructureResolver structureResolver;
        private readonly OperationResolver operationResolver;

        public Qondition resolve(Stack<Token> input, Qontext qontext)
        {
            var type = input.peek().get<Keyword>();
            if (type.type == Keyword.Type.QONDITION_IF)
                return resolveIfElse(input, qontext);
            if (type.type == Keyword.Type.LOOP_DO)
                return resolveDoWhile(input, qontext);
            if (type.type == Keyword.Type.LOOP_WHILE)
                return resolveWhile(input, qontext);
            if (type.type == Keyword.Type.LOOP_FOR)
                return resolveFor(input, qontext);
            return null;
        }

        public IfQondition resolveIfElse(Stack<Token> input, Qontext qontext)
        {
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

            var qondition = new IfQondition(condition, body, qontext);
            if (!input.done && input.peek().raw == Keyword.get(Keyword.Type.QONDITION_ELSE).symbol) {
                qondition.chainElseIf(resolveIfElse(input, qontext));
            }
            return qondition;
        }

        public Qondition resolveDoWhile(Stack<Token> input, Qontext qontext)
        {
            return null;
        }

        public Qondition resolveWhile(Stack<Token> input, Qontext qontext)
        {
            var type = input.digest().get<Keyword>();
            log.spam("resolving " + type.symbol + " condition");

            var condition = operationResolver.resolveOne(
                structureResolver.resolve(input, qontext), qontext);            

            if (input.peek().raw != Structure.get(Structure.Type.BODY).open)
                throw new SqrError("expected { after if, got " + input.peek() + " instead", input.peek());

            var body = new Body(structureResolver.resolve(input, qontext).items);
            return new WhileQondition(condition, body, qontext);
        }

        public Qondition resolveFor(Stack<Token> input, Qontext qontext)
        {
            return null;
        }

        public Qondition resolveTryCatch(Stack<Token> input, Qontext qontext)
        {
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Body
    {
        private static readonly OperationResolver operationResolver = 
            SqrDI.Dependor.get<OperationResolver>(); // use static fields for these kinds of injectors

        protected readonly Token[] content;

        public Body(Token[] content)
        {
            this.content = content;
        }

        internal Stack<Token> getStack()
        {
            return new Stack<Token>(content);
        }

        public OperationResult execute(Qontext qontext)
        {
            var result = new OperationResult();
            var stack = getStack();
            while (!stack.done) {
                var op = operationResolver.resolveOne(stack, qontext);
                var r = op.execute();
                if (op.isReturning) {
                    result.value = r;
                    result.action = OperationResultAction.Return;
                    return result;
                }
                if (op.didContinue) {
                    result.action = OperationResultAction.Continue;
                    return result;
                }
                if (op.didBreak) {
                    result.action = OperationResultAction.Break;
                    return result;
                }
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Body
    {
        private static readonly OperationResolver operationResolver = 
            Dependor.Dependor.get<OperationResolver>(); // use static fields for these kinds of injectors

        protected readonly Token[] content;

        public Body(Token[] content)
        {
            this.content = content;
        }

        public Stack<Token> getStack()
        {
            return new Stack<Token>(content);
        }

        public Value execute(Qontext qontext)
        {
            var stack = getStack();
            while (!stack.done) {
                var op = operationResolver.resolveOne(stack, qontext);
                var r = op.execute();
                if (op.isReturning)
                    return r;
            }
            return null;
        }
    }
}

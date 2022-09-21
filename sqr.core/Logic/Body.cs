using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


using static Qrakhen.Sqr.Core.Operation;

namespace Qrakhen.Sqr.Core
{
    internal class Body
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

        public void execute(Qontext qontext, JumpCallback callback)
        {
            var stack = getStack();
            var statement = Statement.None;
            var result = Value.Void;
            JumpCallback localCallback = (v, s) => { result = v; statement = s; };
            while (!stack.done) {
                var op = operationResolver.resolveOne(stack, qontext);
                op.execute(localCallback);
                if (statement != Statement.None) {
                    callback?.Invoke(result, statement);
                    return;
                }
            }
            callback?.Invoke(Value.Void, Statement.None);
        }
    }
}

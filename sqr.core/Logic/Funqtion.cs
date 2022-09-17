using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Funqtion
    {
        private readonly OperationDigester operationDigester;

        public DeclaredParam[] declaredParameters;
        public Type returnType;
        public Body body;
        
        public virtual Value execute(ProvidedParam[] parameters, Qontext qontext, Value self = null)
        {
            var eq = createExecutionQontext(parameters, qontext);
            if (self != null)
                eq.register("this", self);

            var stack = new Stack<Token>(body.content);
            while (!stack.done)
            {
                var op = operationDigester.digest(stack, eq);
                var r = op.execute();
                if (op.isReturning)
                    return r;
            }
            return null;
        }

        protected Qontext createExecutionQontext(ProvidedParam[] parameters, Qontext qontext)
        {
            var tempQontext = new Qontext(qontext);

            for (int i = 0; i < declaredParameters.Length; i++) {
                var p = declaredParameters[i];
                if (parameters.Length <= i) {
                    if (p.optional) break;
                    else throw new SqrError("parameter " + p.name + " missing");
                } 
                tempQontext.register(parameters[i].name, new Variable(parameters[i].value));
            }

            return tempQontext;
        }

        public struct ProvidedParam
        {
            public string name;
            public Value value;
        }

        public struct DeclaredParam
        {
            public string name;
            public NativeType type;
            public Value defaultValue;
            public bool optional;
        }
    }

    public class InternalFunqtion : Funqtion
    {
        protected Func<ProvidedParam[], Value, Value> callback;

        public InternalFunqtion(Func<ProvidedParam[], Value, Value> callback)
        {
            this.callback = callback;
        }

        public Value execute(ProvidedParam[] parameters, Value self = null)
        {
            return callback(parameters, self);
        }
    }
}

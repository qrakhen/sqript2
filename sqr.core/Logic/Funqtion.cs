using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Funqtion
    {
        private static readonly OperationResolver operationResolver = SqrDI.Dependor.get<OperationResolver>();

        public readonly IDeclareInfo[] parameters = new IDeclareInfo[0];
        public readonly Type returnType;
        public readonly Body body;

        protected Funqtion() { }

        public Funqtion(Body body, IDeclareInfo[] parameters, Type returnType = null)
        {
            this.body = body;
            this.parameters = parameters;
            this.returnType = returnType;
        }
        
        public virtual Value execute(Value[] parameters, Qontext qontext, Value self = null)
        {
            var eq = createExecutionQontext(parameters, qontext);
            if (self != null)
                eq.register("this", self);

            return body.execute(eq);
        }

        protected Qontext createExecutionQontext(Value[] parameters, Qontext qontext)
        {
            var tempQontext = new Qontext(qontext);

            for (int i = 0; i < this.parameters.Length; i++) {
                var p = this.parameters[i];
                if (parameters.Length <= i) {
                    if (p.isOptional) break;
                    else throw new SqrError("parameter " + p.name + " missing");
                } 
                tempQontext.register(this.parameters[i].name, new Variable(parameters[i]));
            }

            return tempQontext;
        }

        public override string ToString()
        {
            return "(" + string.Join(", ", parameters.ToList().Select(_ =>
                (_.type != null ? "@" + _.type.name + " " : "") +
                _.name)) + ")";
        }
    }

    public class InternalFunqtion : Funqtion
    {
        protected Func<Value[], Qontext, Value, Value> callback;

        public InternalFunqtion(Func<Value[], Qontext, Value, Value> callback)
        {
            this.callback = callback;
        }

        public override Value execute(Value[] parameters, Qontext qontext, Value self = null)
        {
            return callback(parameters, qontext, self);
        }

        public Value execute(Value[] parameters, Value self = null)
        {
            return execute(parameters, null, self);
        }
    }
}

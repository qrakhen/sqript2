using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{  
    public class Funqtion : Qontext
    {
        public Parameter[] declaredParameters;
        public Value.Type returnType;
        public Body body;
        
        public virtual Value execute(Value[] parameters)
        {
            var tempQontext = getExecutionQontext(parameters);

            return null;
        }

        protected Qontext getExecutionQontext(Value[] parameters)
        {
            var tempQontext = new Qontext(parent);

            for (int i = 0; i < declaredParameters.Length; i++) {
                var p = declaredParameters[i];
                tempQontext.register(p.name, new Variable(parameters[i]));
            }

            return tempQontext;
        }

        public struct Parameter
        {
            public string name;
            public Value.Type type;
            public Value defaultValue;
            public bool optional;
        }
    }

    public class InternalFunqtion : Funqtion
    {
        protected Func<Value[], Qontext, Value> callback;

        public InternalFunqtion(Qontext parent, Func<Value[], Qontext, Value> callback)
        {
            this.parent = parent;
            this.callback = callback;
        }

        public override Value execute(Value[] parameters)
        {
            return callback(parameters, this);
        }
    }

    public class ExtensionFunqtion : Funqtion
    {
        protected Func<Value[], Value, Value> callback;

        public ExtensionFunqtion(Func<Value[], Value, Value> callback)
        {
            this.callback = callback;
        }

        public Value execute(Value[] parameters, Value self)
        {
            return callback(parameters, self);
        }
    }
}

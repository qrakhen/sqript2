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
        
        public Value execute(Value[] parameters)
        {
            for (int i = 0; i < declaredParameters.Length; i++) {
                var p = declaredParameters[i];
                names[p.name] = new Variable(parameters[i]);
            }

            return null;
        }

        public struct Parameter
        {
            public string name;
            public Value.Type type;
            public Value defaultValue;
            public bool optional;
        }
    }
}

using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Qallable : Value<Funqtion>
    {
        public Qallable(Funqtion value) : base(value, Type.Qallable)
        {
            
        }

        public Value execute(Value[] parameters, Qontext qontext, Value self = null)
        {
            return __value.execute(parameters, qontext, self);
        }
    }
}

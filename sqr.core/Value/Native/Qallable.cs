using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Qallable : Value<Funqtion>
    {
        public readonly Value target;

        public Qallable(Funqtion value, Value target = null) : base(value, Type.Qallable)
        {
            this.target = target;
        }

        public Value execute(Value[] parameters, Qontext qontext)
        {
            return __value.execute(parameters, qontext, target);
        }

        public override string ToString()
        {
            if (__value == null || __value.parameters == null)
                return "Qallable (null)";

            return "(" + string.Join(", ", __value.parameters.ToList().Select(_ => _.name).ToArray()) + " {\n" + "    return " + __value.returnType?.name + ";\n})";
        }
    }
}

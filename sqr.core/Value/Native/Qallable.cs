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
        public Qallable(Funqtion value) : base(value, Type.Qallable)
        {
            xxx("asd");
        }

        public void xxx(String a) { }

        public Value execute(Value[] parameters, Qontext qontext, Value self = null)
        {
            return __value.execute(parameters, qontext, self);
        }

        public override string ToString()
        {
            if (__value == null)
                return "Qallable (null)";

            return "(" + string.Join(", ", __value.parameters.ToList().Select(_ => _.name).ToArray()) + " {\n" + "    return @any;\n})";
        }
    }
}

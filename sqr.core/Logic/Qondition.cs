using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    internal class Qondition
    {
        public readonly Operation condition;
        public readonly Body body;
        public readonly Qontext qontext;

        public Qondition(Operation condition, Body body, Qontext qontext)
        {
            this.condition = condition;
            this.body = body;
            this.qontext = new Qontext(qontext);
        }

        public virtual void execute() { }
    }

    internal class IfQondition : Qondition
    {
        protected IfQondition elseIf;

        public IfQondition(Qondition qondition)
            : base(qondition.condition, qondition.body, qondition.qontext) { }

        public IfQondition(Operation condition, Body body, Qontext qontext) 
            : base(condition, body, qontext) { }

        public IfQondition chainElseIf(IfQondition elseIf)
        {
            this.elseIf = elseIf;
            return this;
        }

        public override void execute() 
        {
            var r = condition?.execute();

            if (r == null || (r as Boolean)) {
                body.execute(qontext);
            } else if (elseIf != null) {
                elseIf.execute();
            }
        }
    }
}

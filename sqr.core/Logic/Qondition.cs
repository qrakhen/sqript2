using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Qrakhen.Sqr.Core.Operation;

namespace Qrakhen.Sqr.Core
{
    internal abstract class Qondition : Injector
    {
        protected readonly Logger log;

        public readonly Operation condition;
        public readonly Body body;
        public readonly Qontext qontext;

        public Qondition(Operation condition, Body body, Qontext qontext)
        {
            this.condition = condition;
            this.body = body;
            this.qontext = new Qontext(qontext);
        }

        public abstract void execute(JumpCallback callback);
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

        public override void execute(JumpCallback callback)
        {
            if (condition == null || condition.execute() as Boolean) {
                body.execute(qontext, callback);
            } else if (elseIf != null) {
                elseIf.execute(callback);
            }         
        }
    }

    internal class WhileQondition : Qondition
    {
        public readonly string name;

        public WhileQondition(Operation condition, Body body, Qontext qontext, string name = null)
                : base(condition, body, qontext) 
        {
            this.name = name;
        }

        public override void execute(JumpCallback callback)
        {
            var statement = Statement.None;
            var result = Value.Void;
            string jumpTarget = null;
            JumpCallback localCallback = (v, s, t ) => { 
                result = v; 
                statement = s; 
                jumpTarget = t; 
            };
            while (condition?.execute() as Boolean) {
                body.execute(qontext, localCallback);
                if (statement == Statement.Return) {
                    log.spam("return jump statement called. value: " + result);
                    callback(result, statement);
                    return;
                }
                if (statement == Statement.Continue) {
                    if (jumpTarget != null && jumpTarget != name) {
                        callback(result, statement, jumpTarget);
                        return;
                    }
                    log.spam("continue jump statement called. value: " + result);
                    qontext.names.clear();
                    continue;
                }
                if (statement == Statement.Break) {
                    if (jumpTarget != null && jumpTarget != name) {
                        callback(result, statement, jumpTarget);
                        return;
                    }
                    log.spam("break jump statement called. value: " + result);
                    break;
                }
            }            
            callback(Value.Void, Statement.None);
        }
    }
}

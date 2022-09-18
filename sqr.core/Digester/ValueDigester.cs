using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Qrakhen.Sqr.Core
{  
    [Injectable]
    public class ValueDigester : Digester<Stack<Token>, Value>
    {
        private readonly Logger log;
        private readonly StructureDigester structureDigester;
        private readonly QollectionDigester qollectionDigester;

        public override Value digest(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);
            Token t = input.peek();
            Value value = null, parent = null;

            if (!t.isType(Token.Type.Value))
                throw new SqrError("token is not a value: " + t);

            string full = "";
            input.process((current, take, index, end) => {
                t = current();
                parent = value; // trace back for calls
                if (!t.isType(Token.Type.Identifier)) {
                    log.spam("got primitive " + t);
                    value = take().makeValue();
                } else {
                    log.spam("got identifier " + t.raw);
                    if (value == null) // root identifier of possible member chain
                        value = qontext.resolveName(take().raw);
                    else
                        value = value.obj.accessMember(take().raw);
                }

                full += t.raw;

                if (input.done)
                    return;

                t = current();
                if (t.isType(Token.Type.Accessor)) {                    
                    log.spam("accessor found: " + t.raw);
                    full += take().raw;
                } else {
                    end();
                }
            });

            log.spam(full + " is a " + value.type.name + " with the value " + value);

            if (!input.done && input.peek().raw == Structure.get(Structure.Type.GROUP).open) {
                log.verbose("funqtion is being called: " + value);
                var parameters = qollectionDigester.digest(
                    structureDigester.digest(
                        input, 
                        qontext),
                    qontext, Structure.get(Structure.Type.GROUP).separator);
                log.spam("parameters: " + parameters);
                value = ((value as Value<Funqtion>).raw as InternalFunqtion).execute(new Funqtion.ProvidedParam[0], parent.obj);
            }

            return value;
        }

        /*
         * public override Value digest(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);
            Token t = input.peek();
            Value value = null;
            Value _member = null;

            if (!t.isType(Token.Type.Value))
                throw new SqrError("token is not a value: " + t);

            if (!t.isType(Token.Type.Identifier)) {
                log.spam("got primitive " + t.makeValue());
                value = input.digest().makeValue();
                if (input.done)
                    return value;

                if (input.peek().isType(Token.Type.Accessor)) {
                    input.digest();
                    _member = value.accessMember(input.digest().raw);
                } else
                    throw new SqrError("weird accessor " + t);
            } else {
                log.spam("got identifier " + t.raw);
                List<string> name = new List<string>();
                input.process(() => input.peek().isType(Token.Type.Identifier), (current, index, abort) => {
                    name.Add(input.digest().value.ToString());
                    if (current() != null && current().isType(Token.Type.Accessor)) {
                        input.digest();
                    }
                });
                log.spam("name " + string.Join(":", name));
                value = qontext.resolveName(name.ToArray());
            }

            // method call?
            if (!input.done && input.peek().raw == Structure.get(Structure.Type.GROUP).open) {
                log.verbose("funqtion is being called: " + _member);
                var parameters = qollectionDigester.digest(
                    structureDigester.digest(
                        input, 
                        qontext),
                    qontext, Structure.get(Structure.Type.GROUP).separator);
                log.spam("parameters: " + parameters);
                value = ((_member as Value<Funqtion>).get() as InternalFunqtion).execute(new Funqtion.ProvidedParam[0], value);
            } else {
                if (_member != null)
                    value = _member;
            }

            return value;
        }*/
    }
}

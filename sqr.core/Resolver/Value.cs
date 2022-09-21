﻿using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using static Qrakhen.Sqr.Core.Operation;

namespace Qrakhen.Sqr.Core
{  
    [Injectable]
    internal class ValueResolver : Resolver<Stack<Token>, Value>
    {
        private readonly Logger log;
        private readonly StructureResolver structureResolver;
        private readonly QollectionResolver qollectionResolver;

        public Value resolve(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);
            Token t = input.peek();

            if (!t.isType(Token.Type.Value))
                throw new SqrError("token is not a value: " + t, t);

            string full = "", member = null;
            Value value = null, parent = null;
            input.process((current, take, index, end) => {

                // get name/literal
                t = current();
                parent = value; // trace back for calls
                if (value == null) {
                    if (!t.isType(Token.Type.Identifier)) {
                        log.spam("got primitive " + t);
                        if (value == null)
                            value = take().makeValue();
                    } else {
                        log.spam("got identifier " + t.raw);
                        if (value == null) // root identifier of possible member chain
                            value = qontext.resolveName(take().raw);
                    }
                } else {
                    member = take().raw;
                }

                // access member if we're beyond first loop
                if (parent != null)
                    value = parent.obj.accessMember(member);

                full += t.raw;

                if (input.done)
                    return;

                // check if it's a function call and resolve
                if (!input.done && input.peek().raw == Structure.get(Structure.Type.GROUP).open) {
                    log.verbose("funqtion is being called: " + value);
                    var parameters = qollectionResolver.resolve(
                        structureResolver.resolve(
                            input,
                            qontext),
                        qontext);
                    log.spam(full + " funq parameters: " + parameters);
                    value = (value.obj as Qallable).execute(parameters.items.ToArray(), qontext);
                    //value = ((value.obj as Value<Funqtion>).raw as InternalFunqtion).execute(new Funqtion.ProvidedParam[0], (parent == null ? value : parent).obj); //@TODO make this less awful
                }

                if (input.done)
                    return;

                // get potential members by scanning for accessor ":"
                t = current();
                if (t.isType(Token.Type.Accessor)) {                    
                    log.spam("accessor found: " + t.raw);
                    full += take().raw;
                } else {
                    end();
                }
            });          

            return value;
        }
    }
}

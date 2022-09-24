using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using static Qrakhen.Sqr.Core.Operation;
using static Qrakhen.Sqr.Core.Token;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    internal class DeclarationResolver : Resolver<Stack<Token>, IDeclareInfo>
    {
        private readonly Logger log;
        private readonly ValueResolver valueResolver;
        private readonly StructureResolver structureResolver;
        private readonly QollectionResolver qollectionResolver;
        private readonly FunqtionResolver funqtionResolver;
        private readonly ObjeqtResolver objeqtResolver;
        private readonly QonditionResolver qonditionResolver;

        public IDeclareInfo resolve(Stack<Token> input, Qontext qontext, bool isFunqtionHeader = false)
        {
            log.verbose("in " + GetType().Name);

            // @type[&]:name
            var info = new IDeclareInfo { };
            info.access = Type.Access.Public;

            // todo: add accessors (easy, just too lazy rn)

            try {

                // check for type or dynamic declaration
                if (input.peek().isType(Token.Type.Type)) {
                    info.type = input.digest().get<Type>();
                    if (info.type != null) {
                        log.spam("detected typed declaration: " + info.type.name);
                    } else {
                        throw new SqrError("unkown type: " + input.peek().raw);
                    }
                } else {
                    if (Runtime.qonfig.forceTypes)
                        throw new SqrTypeError("enforce types is enabled in qonfig. dynamic variables forbidden.");

                    if (!isFunqtionHeader) {
                        var k = input.digest().get<Keyword>();
                        if (k != null && k.isType(Keyword.Type.DECLARE_FUNQTION))
                            info.isFunqtion = true;
                        else if (k != null && !k.isType(Keyword.Type.DECLARE_DYN))
                            throw new SqrError("typed or dynamic declaration expected, got " + k.symbol);
                        else if (k == null)
                            throw new SqrError("typed or dynamic declaration expected");
                        log.spam("dynamic declaration: " + k.symbol);
                    }
                }

                // reference switch &
                if (Validator.Token.isSubType<Operator, Operator.Type>(input.peek(), Operator.Type.LOGIC_AND)) {
                    input.digest();
                    info.isReference = true;
                    log.spam("declared name is a reference");
                }

                // when typed, expect : after @type[&], nah fuck that @todo
                /*if (info.type != null) {
                    var a = input.digest();
                    if (!a.isType(Token.Type.Accessor))
                        throw new SqrError("expected accessor : after typed declaration: " + a, a);
                }*/

                // see if it's a funqtion
                if (Validator.Token.isType(input.peek(), Token.Type.Keyword)) {
                    var k = input.digest().get<Keyword>();
                    if (k.type != Keyword.Type.DECLARE_FUNQTION)
                        throw new SqrError("unexpected keyword at name declaration: " + k, k);
                    info.isFunqtion = true;
                }

                var name = input.digest();
                if (!name.isType(Token.Type.Identifier)) {
                    throw new SqrError("name expected at declaration, got " + name + "instead", name);
                } else {
                    info.name = name.raw;
                }

                if (input.done)
                    return info;

                var op = input.peek().get<Operator>();
                if (op != null && op.isType(Operator.Type.NULLABLE)) {
                    input.digest();
                    info.isOptional = true;
                }

            } catch (Exception e) {
                throw new SqrError("error encountered when trying to parse declaration: " + e.Message, input.peek());
            }
            
            return info;
        }
    }
}

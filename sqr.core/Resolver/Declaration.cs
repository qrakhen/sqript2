using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using static Qrakhen.Sqr.Core.Operation;
using static Qrakhen.Sqr.Core.Token;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class DeclarationResolver : Resolver<Stack<Token>, DeclarationResolver.IDeclareInfo>
    {
        private readonly Logger log;
        private readonly ValueResolver valueResolver;
        private readonly StructureResolver structureResolver;
        private readonly QollectionResolver qollectionResolver;
        private readonly FunqtionResolver funqtionResolver;
        private readonly ObjeqtResolver objeqtResolver;
        private readonly QonditionResolver qonditionResolver;

        public struct IDeclareInfo
        {
            public Type type;
            public Type.Access access;
            public bool isReference;
            public bool isFunqtion;
            public bool isReadonly;
            public string name;
        }

        public IDeclareInfo resolve(Stack<Token> input, Qontext qontext)
        {
            log.spam("in " + GetType().Name);

            // @type[&]:name
            var info = new IDeclareInfo { };
            info.access = Type.Access.Public;

            // todo: add accessors (easy, just too lazy rn)

            // check for type or dynamic declaration
            if (input.peek().isType(Token.Type.Type)) {
                info.type = input.digest().get<Type>();
                if (info.type != null) {                    
                    log.spam("detected typed declaration: " + info.type.name);
                } else {
                    throw new SqrError("unkown type: " + input.peek().raw);
                }
            } else {
                var k = input.digest().get<Keyword>();
                if (k != null && k.isType(Keyword.Type.DECLARE_FUNQTION))
                    info.isFunqtion = true;
                else if (k != null && !k.isType(Keyword.Type.DECLARE_DYN))
                    throw new SqrError("typed or dynamic declaration expected, got " + k.symbol);
                log.spam("dynamic declaration: " + k.symbol);
            }

            // reference switch &
            var op = input.peek().get<Operator>();
            if (op != null) {
                input.digest();
                if (op.type == Operator.Type.LOGIC_AND) info.isReference = true;
                else throw new SqrError("unexpected operator at name declaration: " + op.symbol);
                log.spam("declared name is a reference:" + op.symbol);
            } 

            // when typed, expect : after @type[&], nah fuck that @todo
            if (false && info.type != null) {
                var a = input.digest();
                if (!a.isType(Token.Type.Accessor))
                    throw new SqrError("expected accessor : after typed declaration: " + a, a);
            }

            // see if it's a funqtion
            var f = input.peek();
            if (f.isType(Token.Type.Keyword)) {
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
            
            return info;
        }
    }
}

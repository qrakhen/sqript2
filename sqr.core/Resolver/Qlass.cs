using Qrakhen.SqrDI;
using System.Collections.Generic;
using System.Linq;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    internal class QlassResolver : Resolver<Stack<Token>, Objeqt>
    {
        private readonly ValueResolver valueResolver;
        private readonly StructureResolver structureResolver;
        private readonly DeclarationResolver declarationResolver;
        private readonly FunqtionResolver funqtionResolver;

        public Qlass resolve(Stack<Token> input, Qontext qontext)
        {
            log.verbose("in " + GetType().Name);

            var args = new Type.Args();
            args.methods = new Storage<string, Type.Method>();
            args.fields = new Storage<string, Type.Field>();

            Validator.Token.isSubType<Keyword, Keyword.Type>(input.digest(), Keyword.Type.DECLARE_QLASS, true);

            if (Validator.Token.raw(input.peek(), Token.Type.Identifier, out string name, true)) {
                args.name = input.digest().raw;
                if (Type.get(args.name, qontext) != null) {
                    throw new SqrTypeError("can not redeclare class " + args.name);
                }
                log.spam("declaring new qlass '" + name + "'");
            }

            var body = structureResolver.resolve(input, qontext);

            body.process((current, next, index, end) => 
            {
                var sub = structureResolver.resolveUntil(body, qontext, Token.end);
                var info = declarationResolver.resolve(sub, qontext);
                if (info.isFunqtion) {
                    var fn = structureResolver.resolve(sub, qontext);
                    var funqtion = funqtionResolver.resolve(fn, qontext, info);
                    args.methods[info.name] = new Type.Method(funqtion, info);
                    log.spam("registering method '" + info.name + "' to qlass '" + name + "'");
                } else {
                    if (Validator.Token.tryGetSubType(sub.peek(), Operator.Type.ASSIGN, out Operator value)) {
                        sub.digest();
                        var defaultValue = valueResolver.resolve(sub, qontext);
                        if (!defaultValue.type.isPrimitive) {
                            // throw error fragezeichen?
                        }
                        info.defaultValue = defaultValue;
                    }
                    args.fields[info.name] = new Type.Field(info);
                    log.spam("registering field '" + info.name + "' to qlass '" + name + "'");
                }         
            });

            var type = Type.register(typeof(Instance), args);

            return new Qlass(type);
        }
    }
}

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
    public class OperationResolver : Resolver<Stack<Token>, Operation>
    {
        private readonly Logger log;
        private readonly ValueResolver valueResolver;
        private readonly StructureResolver structureResolver;
        private readonly QollectionResolver qollectionResolver;
        private readonly ObjeqtResolver objeqtResolver;

        public Operation resolve(Stack<Token> input, Qontext qontext)
        {
            return new Operation(build(input, qontext));
        }

        protected Node build(Stack<Token> input, Qontext qontext, Node node = null, int level = 0)
        {
            log.spam("in " + GetType().Name);
            if (input.done)
                return null;

            log.spam("digesting operation at level " + level);
            if (node == null) node = new Node();
            do { 
                log.spam("current node: " + node);
                Token t = input.peek();
                log.spam("token peeked: " + t);

                if (t.isType(Token.Type.Value))
                    handleValue(input, ref node, qontext, level);

                else if (t.isType(Token.Type.Keyword))
                    handleKeyword(input, ref node, qontext, level);

                else if (t.isType(Token.Type.Operator))
                    handleOperator(input, ref node, qontext, level);

                else if (t.isType(Token.Type.Structure))
                    handleStructure(input, ref node, qontext, level);

                else if (t.isType(Token.Type.End))
                    input.digest(); // handleValue(input, node, qontext, level);         
                
                else throw new SqrError("currently unknown token " + t, t);                
            } while (!input.done);

            if (level == 0) {
                if (node.check(011)) {
                    node.left = new Value(Type.Value);
                }
            }

            return node;
        }

        private void handleValue(Stack<Token> input, ref Node node, Qontext qontext, int level = 0)
        {
            Value value = valueResolver.resolve(input, qontext);

            if (!node.put(value)) { 
                throw new SqrError("unexpected value after full operation node " + node, node);
            } else if (node.done && node.op == null) {
                throw new SqrError("unexpected operator-less node " + node, node);
            }
        }

        private void handleKeyword(Stack<Token> input, ref Node node, Qontext qontext, int level = 0)
        {
            var t = input.peek();

            if (!node.empty || level > 0) {
                throw new SqrError("unexpected keyword " + t, t);
            } else {
                var k = input.digest().get<Keyword>();
                if (k.isType(Keyword.Type.DECLARE)) {
                    t = input.digest();
                    if (!t.isType(Token.Type.Identifier)) {
                        throw new SqrError("identifier expected after keyword " + k.symbol + ", got " + t + "instead", t);
                    } else {
                        if (k.isType(Keyword.Type.DECLARE_DYN)) {
                            node.left = qontext.register(t.raw);
                        } else if (k.isType(Keyword.Type.DECLARE_REF)) {
                            node.left = qontext.register(t.raw, null, true);
                        } else {
                            throw new SqrError("not yet implemented: " + k.symbol);
                        }

                        log.spam("registered name " + t + " in qontext");
                    }
                } else if (k.isType(Keyword.Type.FUNQTION_RETURN)) {
                    if (level > 0)
                        throw new SqrError("unexpected return");
                    node.isReturning = true;
                } else {
                    throw new SqrError("not yet implemented: " + t, t);
                }
            }
        }

        private void handleOperator(Stack<Token> input, ref Node node, Qontext qontext, int level = 0)
        {
            var op = input.digest().get<Operator>();

            if (op.isMutator) {

            } else {
                if (node.op == null) {
                    node.op = op;
                } else {
                    if (node.done) {
                        if (op.weight > node.op.weight) {
                            node.right = build(input, qontext, new Node(node.right, null, op), level + 1);
                        } else {
                            node = build(input, qontext, new Node(node, null, op), level + 1);
                        }
                    } else {
                        throw new SqrError("2 operatorss? " + node, node);
                    }
                }
            }
        }

        private void handleStructure(Stack<Token> input, ref Node node, Qontext qontext, int level = 0)
        {
            var t = input.peek();

            if (t.raw != t.get<Structure>().open)
                throw new SqrError("unexpected structure symbol: " + t.raw + ". if anything, structure.open symbol is expected.", t);

            if (node.done)
                throw new SqrError("a structure does not belong here after a done node: " + t.raw + ".", node);

            var innerStack = structureResolver.resolve(input, qontext);
            if (Structure.get(t.raw).type == Structure.Type.QOLLECTION) {
                var qollection = qollectionResolver.resolve(innerStack, qontext);
                node.put(qollection);
            } else if (Structure.get(t.raw).type == Structure.Type.GROUP) {
                var result = resolve(innerStack, qontext).execute();
                node.put(result);
            } else if (Structure.get(t.raw).type == Structure.Type.BODY) {
                var objeqt = objeqtResolver.resolve(innerStack, qontext);
                node.put(objeqt);
            }
        }
    }
}

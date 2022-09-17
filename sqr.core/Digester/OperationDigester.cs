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
    public class OperationDigester : Digester<Stack<Token>, Operation>
    {
        private readonly Logger log;
        private readonly ValueDigester valueDigester;
        private readonly StructureDigester structureDigester;
        private readonly QollectionDigester qollectionDigester;

        public override Operation digest(Stack<Token> input, Qontext qontext)
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
                if (t.isType(Token.Type.Value)) {
                    Value v = valueDigester.digest(input, qontext);
                    handleValue(v, node);
                } else if (t.isType(Token.Type.Keyword)) {
                    if (!node.empty || level > 0) {
                        throw new SqrError("unexpected keyword " + t);
                    } else {
                        var k = input.digest().get<Keyword>();
                        if (k.isType(Keyword.Type.DECLARE)) {
                            t = input.digest();
                            if (!t.isType(Token.Type.Identifier)) {
                                throw new SqrError("identifier expected after keyword " + k.symbol + ", got " + t + "instead");
                            } else {
                                if (k.isType(Keyword.Type.DECLARE_DYN)) {
                                    node = new Node(qontext.register(t.raw), null, null);
                                } else if (k.isType(Keyword.Type.DECLARE_REF)) {
                                    node = new Node(qontext.register(t.raw, null, true), null, null); ;
                                } else {
                                    throw new SqrError("not yet implemented: " + k.symbol);
                                }

                                log.debug("registered " + t + " in qontext");
                            }
                        } else if (k.isType(Keyword.Type.FUNQTION_RETURN)) {
                            if (level > 0)
                                throw new SqrError("unexpected return");
                            node.isReturning = true;
                        } else {
                            throw new SqrError("not yet implemented: " + t);
                        }
                    }
                } else if (t.isType(Token.Type.Operator)) {
                    var op = input.digest().get<Operator>();
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
                            throw new SqrError("2 operatorss? " + node);
                        }
                    }
                } else if (t.isType(Token.Type.Structure)) {
                    if (t.raw != t.get<Structure>().open)
                        throw new SqrError("unexpected structure symbol: " + t.raw + ". if anything, structure.open symbol is expected.");

                    if (node.done)
                        throw new SqrError("a structure does not belong here after a done node: " + t.raw + ".");

                    var innerStack = structureDigester.digest(input, qontext);
                    if (Structure.get(t.raw).type == Structure.Type.QOLLECTION) {
                        var qollection = qollectionDigester.digest(
                            innerStack,
                            qontext, 
                            Structure.get(Structure.Type.QOLLECTION).separator);
                        handleValue(qollection, node);
                    } else if (Structure.get(t.raw).type == Structure.Type.GROUP) {
                        var r = digest(innerStack, qontext).execute();
                        handleValue(r, node);
                    }
                } else if (t.isType(Token.Type.End)) {
                    input.digest();
                    break;
                } else {
                    throw new SqrError("currently unknown token " + t);
                }
            } while (!input.done);

            if (level == 0) {
                if (node.check(011)) {
                    node.left = new Value(Type.Value);
                }
            }

            return node;
        }

        private void handleValue(Value value, Node node)
        {            
            if (!node.put(value)) { 
                throw new SqrError("unexpected value after full operation node " + node);
            } else if (node.done && node.op == null) {
                throw new SqrError("unexpected operator-less node " + node);
            }
        }
    }
}

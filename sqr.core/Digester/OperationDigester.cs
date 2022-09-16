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
    public class OperationDigester : Digester<Stack<Token>, Node>
    {
        private readonly Logger log;
        private readonly ValueDigester valueDigester;
        private readonly ValueDigester structureDigester;

        public Node digest(Stack<Token> input, Qontext qontext, Node node = null, int level = 0)
        {
            if (input.done)
                return null;

            if (node == null) node = new Node();
            do { 
                log.debug(node);
                Token t = input.peek();
                if (t.isType(Token.Type.Value)) {
                    Value v = valueDigester.digest(input, qontext);
                    if (!node.put(v)) {
                        throw new SqrError("unexpected value after full operation node " + node);
                    } else if (node.done && node.op == null) {
                        throw new SqrError("unexpected operator-less node " + node);
                    }
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
                                    node = new Node(qontext.register(t), null, null);
                                } else if (k.isType(Keyword.Type.DECLARE_REF)) {
                                    node = new Node(qontext.register(t, Value.Type.None, null, true), null, null);
                                } else {
                                    throw new SqrError("not yet implemented: " + k.symbol);
                                }
                            }
                        } else {
                            throw new SqrError("not yet implemented: " + t);
                        }
                    }
                } else if (t.isType(Token.Type.Operator)) {
                    var op = input.digest().get<Operator>();
                    if (node.op == null) {
                        node.op = op;
                        if (level == 0) {
                            if (op.type == Operator.Type.ASSIGN) {

                            }
                        }
                    } else {
                        if (node.done) {
                            if (op.weight > node.op.weight) {
                                node.right = digest(input, qontext, new Node(node.right, null, op), level + 1);
                            } else {
                                node = digest(input, qontext, new Node(node, null, op), level + 1);
                            }
                        } else {
                            throw new SqrError("2 ops? " + node);
                        }
                    }
                } else if (
                        t.isType(Token.Type.Structure) &&
                        t.get<string>() == Structure.get(Structure.Type.GROUP).open) {
                    if (node.done) log.error("nap");
                    //var n = digest();
                } else if (t.isType(Token.Type.End)) {
                    input.digest();
                    break;
                }
            } while (!input.done);
            return node;
        }
    }
}

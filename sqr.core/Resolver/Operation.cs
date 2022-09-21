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
    internal class OperationResolver : Resolver<Stack<Token>, Operation>
    {
        private readonly Logger log;
        private readonly ValueResolver valueResolver;
        private readonly StructureResolver structureResolver;
        private readonly QollectionResolver qollectionResolver;
        private readonly FunqtionResolver funqtionResolver;
        private readonly ObjeqtResolver objeqtResolver;
        private readonly QonditionResolver qonditionResolver;
        private readonly DeclarationResolver declarationResolver;
               
        public Operation resolveOne(Stack<Token> input, Qontext qontext)
        {
            Statement statement = Statement.None;

            if ((    
                    input.peek().type == Token.Type.Keyword && 
                    input.peek().get<Keyword>().type == Keyword.Type.FUNQTION_RETURN) || ((
                    input.peek().type == Token.Type.Operator &&
                    input.peek().get<Operator>().type == Operator.Type.ASSIGN))) {
                input.digest();
                statement = Statement.Return;
            }
            if (
                    input.peek().type == Token.Type.Keyword &&
                    input.peek().get<Keyword>().type == Keyword.Type.LOOP_CONTINUE) {
                input.digest();
                return new Operation(new Node(), Statement.Continue);
            }
            if (
                    input.peek().type == Token.Type.Keyword &&
                    input.peek().get<Keyword>().type == Keyword.Type.LOOP_BREAK) {
                input.digest();
                return new Operation(new Node(), Statement.Break);
            }
            
            return new Operation(build(input, qontext), statement);
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

                else if (t.isType(Token.Type.Keyword | Token.Type.Type))
                    handleKeyword(input, ref node, qontext, level);

                else if (t.isType(Token.Type.Operator))
                    handleOperator(input, ref node, qontext, level);

                else if (t.isType(Token.Type.Structure))
                    handleStructure(input, ref node, qontext, level);

                else if (input.peek().isType(Token.Type.End)) {
                    if (level == 0) input.digest();
                    break;
                } else throw new SqrError("unknown or entirely unexpected token " + t, t);

                if (node.left is Qondition) // conditions are single-value nodes
                    break;

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
                var k = input.peek().get<Keyword>();
                if (k != null && k.isType(Keyword.Type.QONDITION)) {
                    node.left = qonditionResolver.resolve(input, qontext);
                } else {
                    var info = declarationResolver.resolve(input, qontext);
                    if (info.isFunqtion) {
                        var funqtion = funqtionResolver.resolve(
                            structureResolver.resolve(input, qontext), qontext, info);
                        node.left = qontext.register(info.name, new Qallable(funqtion));
                    } else {
                        node.left = qontext.register(info.name, null, info.isReference, info.type, info.isReadonly);
                    }
                    log.spam("registered name " + t + " in qontext");
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
                //@todo: callback könnte hier zu bugs führen. brauchen wir returns in () klammern?
                var result = resolveOne(innerStack, qontext); //, callback);//.execute(); // we dont have to execute right away. why would we do that even.
                node.put(result.head);
            } else if (Structure.get(t.raw).type == Structure.Type.BODY) {
                var objeqt = objeqtResolver.resolve(innerStack, qontext);
                node.put(objeqt);
            }
        }
    }
}

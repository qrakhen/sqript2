using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    internal class Operation : Injector
    {
        protected readonly Logger log;

        public Node head { get; protected set; }
        public bool isReturning { get; protected set; }
        public bool didContinue { get; protected set; }
        public bool didBreak { get; protected set; }

        public Operation(Node head = null, bool isReturning = false, bool didContinue = false, bool didBreak = false)
        {
            log.spam(head.render());
            this.head = head;
            this.isReturning = isReturning;
            this.didContinue = didContinue;
            this.didBreak = didBreak;
        }

        public Value execute()
        {
            log.spam("executing operation");
            if (head == null)
                return null;
            return head.execute();
        }

        public class Node
        {
            public object left;
            public object right;
            public Operator op;

            public Node(object left = null, object right = null, Operator op = null)
            {
                this.left = left;
                this.right = right;
                this.op = op;
            }

            public Value execute()
            {
                if (left != null) {
                    if (op != null && right != null) 
                    {
                        Value _right;
                        if (right is Node) _right = (right as Node).execute();
                        else _right = (Value)right;

                        Value _left;
                        if (left is Node) _left = (left as Node).execute();
                        else _left = (Value)left;

                        if (op.isType(Operator.Type.ASSIGN_REF)) {

                        } else if (op.isType(Operator.Type.ASSIGN)) {

                        } else {
                            if (_left is Variable)
                                _left = (_left as Variable).get();
                            if (_right is Variable)
                                _right = (_right as Variable).get();
                        }

                        Logger.TEMP_STATIC_DEBUG.spam("resolving " + this);
                        Logger.TEMP_STATIC_DEBUG.spam("result " + op.resolve(_left, _right));

                        return op.resolve(_left, _right);
                    } else {
                        return (left is Node ? (left as Node).execute() : (Value)left);
                    }
                } else {
                    return null;
                }
            }

            public bool done => (left != null && right != null && op != null);
            public bool empty => (left == null && right == null && op == null);

            public bool check(int pattern)
            {
                return (pattern == (left != null ? 100 : 0) + (op != null ? 10 : 0) + (right != null ? 1 : 0));
            }

            public bool put(object value)
            {
                if (left == null) left = value;
                else if (right == null) right = value;
                else return false;
                return true;
            }

            public override string ToString()
            {
                if (empty) return "{ empty }";
                return "{ " + (left ?? "null") + " " + (op == null ? "noop" : op.symbol) + " " + (right ?? "null") + " }";
            }

            public string render()
            {
                var drawer = new StringDrawer(Console.WindowWidth - 32, Console.WindowHeight / 4);
                var x = drawer.length / 2;
                var y = drawer.height - 2;
                __render(x, y, drawer);
                return drawer.render();
            }

            private void __render(int x = 0, int y = 0, StringDrawer drawer = null)
            {
                var e = "";
                //drawer.draw(x, y, op?.symbol ?? " ");
                if (left is Node) {
                    (left as Node).__render(x - 7, y - 1, drawer);
                } else {
                    var s = left?.ToString();
                    if (s != null) {
                        e += "(" + s + " ";
                        //drawer.draw(x - s.Length - 2, y, s ?? " ");
                        //drawer.draw(x - s.Length - 3, y, "("); 
                    }
                }

                e += op?.symbol;

                if (right is Node) {
                    (right as Node).__render(x + 7, y - 1, drawer);
                } else {
                    var s = right?.ToString();
                    if (s != null) {
                        e += " " + s + ")";
                        //drawer.draw(x + 3, y, s ?? " ");
                        //drawer.draw(x + 4 + s.Length, y, ")");
                    }
                }
                drawer.draw(x - e.Length / 2, y, e);
            }
        }        
    }

    public struct OperationResult
    {
        public Value value;
        public OperationResultAction action;
    }

    public enum OperationResultAction
    {
        None = default,
        Return,
        Break,
        Continue
    }
}

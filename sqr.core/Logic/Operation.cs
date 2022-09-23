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
        public Statement statement { get; protected set; }
        public string jumpTarget { get; protected set; }

        public Operation(Node head = null, Statement statement = Statement.None, string jumpTarget = null)
        {
            this.head = head;
            this.statement = statement;
            this.jumpTarget = jumpTarget;
        }

        public void execute(JumpCallback callback, Qontext qontext) 
        {
            log.spam("executing operation");
            if (head == null) {
                callback?.Invoke(Value.Void, Statement.None);
                return;
            }

            if (statement == Statement.Continue || statement == Statement.Break) {
                callback?.Invoke(Value.Void, statement, jumpTarget);
            } else {
                if (head.left is Qondition) {
                    (head.left as Qondition).execute(callback);
                    return;
                } else {
                    var value = head.execute(qontext);
                    callback?.Invoke(value, statement, jumpTarget);
                    return;
                }
            }
        }

        // not all callers use callback, so we have both return type here as a QoL
        public Value execute(Qontext qontext)
        {
            Value value = Value.Void;
            execute((v, s, t) => { value = v; }, qontext);
            return value;            
        }

        public delegate void JumpCallback(Value value, Statement statement = Statement.None, string jumpTarget = null);

        public enum Statement
        {
            None,
            Return,
            Continue,
            Break
        }

        public class Node
        {
            public object left;
            public Operator leftMod;
            public object right;
            public Operator rightMod;
            public object data;
            public Operator op;

            public Node(object left = null, object right = null, Operator op = null)
            {
                this.left = left;
                this.right = right;
                this.op = op;
            }

            public Value execute(Qontext qontext)
            {
                if (left != null) {
                    Value _left = resolveValue(left, leftMod, qontext);
                    if (op != null && right != null) {
                        Value _right = resolveValue(right, rightMod, qontext);

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

                        var result = op.resolve(_left, _right);                        
                        return resolveValue(result, leftMod, qontext);
                    } else {
                        return _left;
                    }
                } else {
                    return null;
                }
            }

            private Value resolveValue(object value, Operator mod, Qontext qontext)
            {
                Value _value;
                if (value is Node) {
                    _value = (value as Node).execute(qontext);
                } else {
                    _value = value as Value;
                }

                if (mod != null) {
                    _value = mod.resolve(null, _value);
                }
                
                if (_value.obj is Qallable && data != null) {
                    return (_value.obj as Qallable).execute((data as Qollection).items.ToArray(), qontext);
                } else {
                    return _value;
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
}

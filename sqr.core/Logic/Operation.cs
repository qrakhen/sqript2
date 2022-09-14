using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Operation
    {
        public Node head { get; protected set; }

        public Operation(Node head = null)
        {
            this.head = head;
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
                    if (op != null && right != null) {
                        Value _left;
                        Value _right;
                        if (right is Node) _right = (right as Node).execute();
                        else _right = (Value)right;
                        if (left is Node) _left = (left as Node).execute();
                        else _left = (Value)left;
                        return op.resolve(_left, _right);
                    } else {
                        return (Value)left;
                    }
                } else {
                    return null;
                }
            }

            public bool done => (left != null && right != null && op != null);
            public bool empty => (left == null && right == null && op == null);

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
        }
    }
}

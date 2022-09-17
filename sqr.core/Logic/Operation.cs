﻿using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Operation : Injector
    {
        protected readonly Logger log;

        public Node head { get; protected set; }
        public bool isReturning => head?.isReturning ?? false;

        public Operation(Node head = null)
        {
            this.head = head;
        }

        public Value execute()
        {
            log.spam("executing operation");
            return head.execute();
        }

        public class Node
        {
            public bool isReturning;
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
                        else if (right is Variable) _right = (right as Variable).get();
                        else _right = (Value)right;

                        Value _left;
                        if (left is Node) _left = (left as Node).execute();
                        else if (left is Variable && !op.isType(Operator.Type.ASSIGN | Operator.Type.ASSIGN_REF)) _left = (left as Variable).get();
                        else _left = (Value)left;

                        Logger.TEMP_STATIC_DEBUG.spam("resolving " + this);
                        Logger.TEMP_STATIC_DEBUG.spam("result " + op.resolve(_left, _right));

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

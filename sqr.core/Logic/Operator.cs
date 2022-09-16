using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Operator : ITyped<Operator.Type>
    {
        private static readonly Storage<Type, Operator> operators = new Storage<Type, Operator>();

        public string symbol;
        public int weight;
        public Func<Value, Value, Value> resolve;

        public Operator(Type type, string symbol, int weight, Func<Value, Value, Value> resolve)
        {
            this.type = type;
            this.symbol = symbol;
            this.weight = weight;
            this.resolve = resolve;
        }

        public override string ToString()
        {
            return symbol;
        }

        public static Operator get(string symbol)
        {
            return operators.findOne(_ => _.symbol == symbol);
        }

        public enum Type
        {
            CALC_ADD,
            CALC_SUB,
            CALC_MULT,
            CALC_DIV,
            COND_AND,
            COND_OR,
            COMP_EQUAL,
            COMP_NOTEQUAL,
            COMP_GT,
            COMP_GTEQUAL,
            COMP_LT,
            COMP_LTEQUAL,
            LOGIC_NOT,
            LOGIC_OR,
            LOGIC_AND,
            LOGIC_XOR,
            LIST_ADD,
            ASSIGN,
            ASSIGN_REF
        }

        public static void register(Type type, string symbol, int weight, Func<Value, Value, Value> resolve)
        {
            operators.Add(type, new Operator(type, symbol, weight, resolve));
        }

        static Operator()
        {   
            register(Type.CALC_ADD, "+", 2, (left, right) => {
                return new Value(Value.Type.Number, left.asNumber() + right.asNumber());  
            });

            register(Type.CALC_SUB, "-", 2, (left, right) => {
                return new Value(Value.Type.Number, left.asNumber() - right.asNumber());
            });

            register(Type.CALC_MULT, "*", 4, (left, right) => {
                return new Value(Value.Type.Number, left.asNumber() * right.asNumber());
            });

            register(Type.CALC_DIV, "/", 4, (left, right) => {
                return new Value(Value.Type.Number, left.asNumber() / right.asNumber());
            });

            register(Type.COND_AND, "&&", 1, (left, right) => {
                return new Value(Value.Type.Boolean, left.asBoolean() && right.asBoolean());
            });

            register(Type.COND_OR, "||", 1, (left, right) => {
                return new Value(Value.Type.Boolean, left.asBoolean() || right.asBoolean());
            });

            register(Type.COMP_EQUAL, "==", 1, (left, right) => {
                return new Value(Value.Type.Boolean, (left.rawValue.Equals(right.rawValue)));
            });

            register(Type.COMP_NOTEQUAL, "!=", 1, (left, right) => {
                return new Value(Value.Type.Boolean, (!left.rawValue.Equals(right.rawValue)));
            });

            register(Type.COMP_GT, ">", 1, (left, right) => {
                return new Value(Value.Type.Boolean, left.asNumber() > right.asNumber());
            });

            register(Type.COMP_GTEQUAL, ">=", 1, (left, right) => {
                return new Value(Value.Type.Boolean, left.asNumber() >= right.asNumber());
            });

            register(Type.COMP_LT, "<", 1, (left, right) => {
                return new Value(Value.Type.Boolean, left.asNumber() < right.asNumber());
            });

            register(Type.COMP_LTEQUAL, "<=", 1, (left, right) => {
                return new Value(Value.Type.Boolean, left.asNumber() <= right.asNumber());
            });

            register(Type.LIST_ADD, "<+", 0, (left, right) => {
                (left.rawValue as Qollection).add(right);
                return left;
            });

            register(Type.ASSIGN, "<~", 0, (left, right) => {
                left.set(right);
                return left;
            });

            register(Type.ASSIGN_REF, "<&", 0, (left, right) => {
                if (!left.isReference)
                    throw new SqrError("can not pass value " + right + "'s reference to " + left + ": not a reference");
                left.set(right);
                return left;
            });

            register(Type.LOGIC_NOT, "!", 0, (left, right) => {
                return null;
            });
        }
    }
}

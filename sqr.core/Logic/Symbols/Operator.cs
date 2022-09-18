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
        public bool isMutator;
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

        [Flags]
        public enum Type
        {
            CALC_ADD = 1,
            CALC_SUB = 2,
            CALC_MULT = 4,
            CALC_DIV = 8,
            CALC = CALC_ADD | CALC_SUB | CALC_MULT | CALC_DIV,
            COND_AND = 16,
            COND_OR = 32,
            COMP_EQUAL = 64,
            COMP_NOTEQUAL = 128,
            COMP_GT = 246,
            COMP_GTEQUAL = 512,
            COMP_LT = 1024,
            COMP_LTEQUAL = 2048,
            LOGIC_NOT = 4096,
            LOGIC_OR = 8192,
            LOGIC_AND = 16384,
            LOGIC_XOR = 32768,
            QOLLECTION_ADD = 65536,
            ASSIGN = 131072,
            ASSIGN_REF = 262144
        }

        public static void register(Type type, string symbol, int weight, Func<Value, Value, Value> resolve)
        {
            operators.Add(type, new Operator(type, symbol, weight, resolve));
        }

        static Operator()
        {   
            register(Type.CALC_ADD, "+", 2, (left, right) => {
                if (left is Number && right is Number)
                    return new Number((left as Number) + (right as Number));
                return new Number(0);
            });

            register(Type.CALC_SUB, "-", 2, (left, right) => {
                if (left is Number && right is Number)
                    return new Number((left as Number) - (right as Number));
                return new Number(0);
            });

            register(Type.CALC_MULT, "*", 4, (left, right) => {
                if (left is Number && right is Number)
                    return new Number((left as Number) * (right as Number));
                return new Number(0);
            });

            register(Type.CALC_DIV, "/", 4, (left, right) => {
                if (left is Number && right is Number)
                    return new Number((left as Number) / (right as Number));
                return new Number(0);
            });

            register(Type.COND_AND, "&&", 1, (left, right) => {
                return new Boolean((left as Boolean) && (right as Boolean));
            });

            register(Type.COND_OR, "||", 1, (left, right) => {
                return new Boolean((left as Boolean) && (right as Boolean));
            });

            register(Type.COMP_EQUAL, "==", 1, (left, right) => {
                return new Boolean(left.Equals(right));
            });

            register(Type.COMP_NOTEQUAL, "!=", 1, (left, right) => {
                return new Boolean(!left.Equals(right));
            });

            register(Type.COMP_GT, ">", 1, (left, right) => {
                return new Boolean((left as Number) > (right as Number));
            });

            register(Type.COMP_GTEQUAL, ">=", 1, (left, right) => {
                return new Boolean((left as Number) >= (right as Number));
            });

            register(Type.COMP_LT, "<", 1, (left, right) => {
                return new Boolean((left as Number) < (right as Number));
            });

            register(Type.COMP_LTEQUAL, "<=", 1, (left, right) => {
                return new Boolean((left as Number) <= (right as Number));
            });

            register(Type.QOLLECTION_ADD, "<+", 0, (left, right) => {
                (left as Qollection).add(right);
                return left;
            });

            register(Type.ASSIGN, "<~", 0, (left, right) => {
                (left as Variable).set(right, false);
                return left;
            });

            register(Type.ASSIGN_REF, "<&", 0, (left, right) => {
                (left as Variable).set((right as Variable), true);
                return left;
            });

            register(Type.LOGIC_NOT, "!", 0, (left, right) => {
                return new Boolean(!(right as Boolean));
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    internal class Operator : ITyped<Operator.Type>
    {
        private static readonly Storage<Type, Operator> operators = new Storage<Type, Operator>();

        public string symbol;
        public int weight;
        public bool isMutator;
        public Func<Value, Value, Value> resolve;
        public List<string> aliases = new List<string>();

        public Operator(Type type, string symbol, int weight, Func<Value, Value, Value> resolve)
        {
            this.type = type;
            this.symbol = symbol;
            this.weight = weight;
            this.resolve = resolve;
            alias(symbol);
        }

        public Operator alias(params string[] alias)
        {
            aliases = aliases.Concat(alias).ToList();
            return this;
        }

        public override string ToString()
        {
            return symbol;
        }

        public static Operator get(string symbol)
        {
            return operators.findOne(_ => _.aliases.Contains(symbol)).Value;
        }

        [Flags]
        public enum Type
        {
            CALC_ADD = BitFlag._1,
            CALC_SUB = BitFlag._2,
            CALC_MULT = BitFlag._3,
            CALC_DIV = BitFlag._4,
            CALC = CALC_ADD | CALC_SUB | CALC_MULT | CALC_DIV,
            COND_AND = BitFlag._5,
            COND_OR = BitFlag._6,
            COMP_EQUAL = BitFlag._7,
            COMP_NOTEQUAL = BitFlag._8,
            COMP_GT = BitFlag._9,
            COMP_GTEQUAL = BitFlag._10,
            COMP_LT = BitFlag._11,
            COMP_LTEQUAL = BitFlag._12,
            LOGIC_NOT = BitFlag._13,
            LOGIC_OR = BitFlag._14,
            LOGIC_AND = BitFlag._15,
            LOGIC_XOR = BitFlag._16,
            QOLLECTION_ADD = BitFlag._17,
            ASSIGN = BitFlag._18,
            ASSIGN_REF = BitFlag._19,
            NULLABLE = BitFlag._20,
            ACCESSOR = BitFlag._21
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
                if (left is String)
                    return new String((left.raw as String) + (right.raw));
                if (right is String)
                    return new String((left.raw) + (right.raw as String));
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

            register(Type.COND_AND, "&&", 0, (left, right) => {
                return new Boolean((left as Boolean) && (right as Boolean));
            });

            register(Type.COND_OR, "||", 0, (left, right) => {
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

            register(Type.NULLABLE, "?", 0, (left, right) => {
                return null;
            });

            register(Type.ACCESSOR, ":", 8, (left, right) => {
                return left.accessMember(right as String);
            });

            register(Type.LOGIC_NOT, "!", 0, (left, right) => {
                return new Boolean(!(right as Boolean));
            });

            register(Type.LOGIC_AND, "&", 0, (left, right) => {
                if (left is Number && right is Number)
                    return new Number((left as Number).asInteger() & (right as Number).asInteger());
                return new Number(0);
            });

            register(Type.LOGIC_OR, "|", 0, (left, right) => {
                if (left is Number && right is Number)
                    return new Number((left as Number).asInteger() | (right as Number).asInteger());
                return new Number(0);
            });

            register(Type.LOGIC_XOR, "^", 0, (left, right) => {
                if (left is Number && right is Number)
                    return new Number((left as Number).asInteger() ^ (right as Number).asInteger());
                return new Number(0);
            });
        }
    }
}

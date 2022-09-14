using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Keyword
    {
        private static readonly Storage<Type, Keyword> keywords = new Storage<Type, Keyword>();

        public Type type;
        public string symbol;
        public Func<object, Stack<Token>> resolve;

        public static Keyword get(string symbol)
        {
            return keywords.findOne(_ => _.symbol == symbol);
        }

        public enum Type
        {
            IMPORT,
            QONDITION_IF,
            QONDITION_ELSE,
            LOOP_FOR,
            LOOP_WHILE,
            LOOP_DO,
            FUNQTION_RETURN,
            FUNQTION_DECLARE,
            QLASS_DECLARE,
        }

        public const Type IMPORT = Type.IMPORT;
        public const Type QONDITION_IF = Type.QONDITION_IF;
        public const Type QONDITION_ELSE = Type.QONDITION_ELSE;
        public const Type LOOP_FOR = Type.LOOP_FOR;
        public const Type LOOP_WHILE = Type.LOOP_WHILE;
        public const Type LOOP_DO = Type.LOOP_DO;
        public const Type FUNQTION_RETURN = Type.FUNQTION_RETURN;
        public const Type FUNQTION_DECLARE = Type.FUNQTION_DECLARE;
        public const Type QLASS_DECLARE = Type.QLASS_DECLARE;
    }
}

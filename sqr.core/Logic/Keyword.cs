using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Keyword : ITyped<Keyword.Type>
    {
        private static readonly Storage<Type, Keyword> keywords = new Storage<Type, Keyword>();

        public string symbol;
        public Func<object, Stack<Token>> resolve;

        public Keyword(Type type, string symbol)
        {
            this.type = type;
            this.symbol = symbol;
        }

        public override string ToString()
        {
            return symbol;
        }

        public static Keyword get(string symbol)
        {
            return keywords.findOne(_ => _.symbol == symbol);
        }

        [Flags]
        public enum Type
        {
            DECLARE_DYN,
            DECLARE_REF,
            DECLARE_FUNQTION,
            DECLARE_QLASS,
            DECLARE = DECLARE_DYN | DECLARE_REF | DECLARE_FUNQTION | DECLARE_QLASS,
            IMPORT,
            QONDITION_IF,
            QONDITION_ELSE,
            LOOP_FOR,
            LOOP_WHILE,
            LOOP_DO,
            FUNQTION_RETURN,
        }

        public static void register(Type type, string symbol)
        {
            keywords.Add(type, new Keyword(type, symbol));
        }

        static Keyword()
        {
            register(Type.DECLARE_DYN, "var"); 
            register(Type.DECLARE_REF, "ref");
            register(Type.DECLARE_FUNQTION, "funqtion");
            register(Type.DECLARE_QLASS, "qlass");
            register(Type.IMPORT, "import");
            register(Type.QONDITION_IF, "if");
            register(Type.QONDITION_ELSE, "else");
            register(Type.LOOP_FOR, "for");
            register(Type.LOOP_WHILE, "while");
            register(Type.LOOP_DO, "do");
            register(Type.FUNQTION_RETURN, "return");
        }
    }
}

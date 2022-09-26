using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    internal class Keyword : ITyped<Keyword.Type>
    {
        private static readonly Storage<Type, Keyword> keywords = new Storage<Type, Keyword>();

        public string symbol;
        public List<string> aliases = new List<string>();

        public Keyword(Type type, string symbol)
        {
            this.type = type;
            this.symbol = symbol;
            alias(symbol);
        }

        public Keyword alias(params string[] alias)
        {
            aliases = aliases.Concat(alias).ToList();
            return this;
        }

        public override string ToString()
        {
            return symbol;
        }

        public static Keyword get(string symbol)
        {
            if (symbol.StartsWith("@")) return keywords[Type.DECLARE_TYPED];
            return keywords.findOne(_ => _.aliases.Contains(symbol)).Value;
        }

        public static Keyword get(Type type)
        {
            return keywords[type];
        }

        [Flags]
        public enum Type
        {
            DECLARE_DYN = BitFlag._1,
            DECLARE_TYPED = BitFlag._3,
            DECLARE_FUNQTION = BitFlag._4,
            DECLARE_QLASS = BitFlag._5,
            DECLARE = DECLARE_DYN | DECLARE_FUNQTION | DECLARE_QLASS,
            QONDITION_IF = BitFlag._7,
            QONDITION_ELSE = BitFlag._8,
            LOOP_FOR = BitFlag._9,
            LOOP_WHILE = BitFlag._10,
            LOOP_DO = BitFlag._11,
            LOOP_BREAK = BitFlag._12,
            LOOP_CONTINUE = BitFlag._13,
            QONDITION = QONDITION_IF | QONDITION_ELSE | LOOP_DO | LOOP_FOR | LOOP_WHILE,
            FUNQTION_RETURN = BitFlag._14,
            INSTANCE_CREATE = BitFlag._15,
            INSTANCE_THIS = BitFlag._16,
            IMPORT = BitFlag._17,
            EXPORT = BitFlag._18,
            MODULE = BitFlag._19,
            FUNQTION_INLINE = BitFlag._20
        }

        public static Keyword register(Type type, string symbol)
        {
            var word = new Keyword(type, symbol);
            keywords.Add(type, word);
            return word;
        }

        static Keyword()
        {
            register(Type.DECLARE_DYN, "var")
                .alias("*~");
            /*register(Type.INSTANCE_THIS, "this")
                .alias(".");*/
            register(Type.DECLARE_TYPED, "@");
            register(Type.DECLARE_FUNQTION, "funqtion")
                .alias("funq", "fq", "function");
            register(Type.FUNQTION_INLINE, "~:");
            register(Type.DECLARE_QLASS, "qlass");
            //register(Type.IMPORT, "import");
            register(Type.EXPORT, "export");
            register(Type.MODULE, "module");
            register(Type.QONDITION_IF, "if")
                .alias("~?");
            register(Type.QONDITION_ELSE, "else")
                .alias("?~");
            register(Type.LOOP_FOR, "for");
            register(Type.LOOP_WHILE, "while")
                .alias("as");
            register(Type.LOOP_DO, "do");
            register(Type.LOOP_BREAK, "break");
            register(Type.LOOP_CONTINUE, "continue")
                .alias("~^"); 
            register(Type.FUNQTION_RETURN, "return")
                .alias("<:");
            register(Type.INSTANCE_CREATE, "new")
                .alias("*:", "spawn");
        }
    }
}

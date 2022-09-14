using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Structure
    {
        private static readonly Storage<Type, Structure> structures = new Storage<Type, Structure>();

        public readonly Type type;
        public readonly string open;
        public readonly string close;

        public Structure(Type type, string open, string close)
        {
            this.type = type;
            this.open = open;
            this.close = close;
        }

        public static Structure get(string symbol)
        {
            return structures.findOne(_ => _.open == symbol);
        }

        public override string ToString()
        {
            return open + " " + close;
        }

        public enum Type
        {
            BODY,
            LIST,
            GROUP
        }

        public static void register(Type type, string open, string close)
        {
            structures.Add(type, new Structure(type, open, close));
        }

        static Structure()
        {
            register(Type.BODY, "{", "}");
            register(Type.LIST, "[", "]");
            register(Type.GROUP, "(", ")");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Structure : ITyped<Structure.Type>
    {
        private static readonly Storage<Type, Structure> structures = new Storage<Type, Structure>();

        public readonly string open;
        public readonly string close;
        public readonly string extra;

        public Structure(Type type, string open, string close, string extra = null)
        {
            this.type = type;
            this.open = open;
            this.close = close;
            this.extra = extra ?? "";
        }

        public static Structure get(string symbol)
        {
            return structures.findOne(_ => _.open == symbol || _.close == symbol || _.extra == symbol);
        }

        public static Structure get(Type type)
        {
            return structures[type];
        }

        public override string ToString()
        {
            return open + " " + close;
        }

        public enum Type
        {
            BODY,
            QOLLECTION,
            ARRAY,
            GROUP
        }

        public static void register(Type type, string open, string close, string extra = null)
        {
            structures.Add(type, new Structure(type, open, close, extra));
        }

        static Structure()
        {
            register(Type.BODY, "{", "}", ",");
            register(Type.QOLLECTION, "[", "]", ",");
            register(Type.GROUP, "(", ")", ",");
        }
    }
}

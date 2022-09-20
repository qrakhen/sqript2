using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Structure : ITyped<Structure.Type>
    {
        private static readonly Storage<Type, Structure> structures = new Storage<Type, Structure>();

        public static List<string> openers => structures.Values.Select(_ => _.open).ToList();
        public static List<string> closers => structures.Values.Select(_ => _.close).ToList();

        public readonly string open;
        public readonly string close;
        public readonly string separator;

        public Structure(Type type, string open, string close, string separator = null)
        {
            this.type = type;
            this.open = open;
            this.close = close;
            this.separator = separator ?? "";
        }

        public static Structure get(string symbol)
        {
            return structures.findOne(_ => _.open == symbol || _.close == symbol || _.separator == symbol);
        }

        public static Structure get(Type type)
        {
            return structures[type];
        }

        public override string ToString()
        {
            return open + " " + close;
        }

        [Flags]
        public enum Type
        {
            BODY,
            QOLLECTION,
            ARRAY,
            GROUP
        }

        public static void register(Type type, string open, string close, string separator = null)
        {
            structures.Add(type, new Structure(type, open, close, separator));
        }

        static Structure()
        {
            register(Type.BODY, "{", "}", ",");
            register(Type.QOLLECTION, "[", "]", ",");
            register(Type.GROUP, "(", ")", ",");
        }
    }
}

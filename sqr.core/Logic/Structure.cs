﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Structure : ITyped<Structure.Type>
    {
        private static readonly Storage<Type, Structure> structures = new Storage<Type, Structure>();

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
            OBJEQT,
            QOLLECTION,
            GROUP
        }

        public static void register(Type type, string open, string close)
        {
            structures.Add(type, new Structure(type, open, close));
        }

        static Structure()
        {
            register(Type.OBJEQT, "{", "}");
            register(Type.QOLLECTION, "[", "]");
            register(Type.GROUP, "(", ")");
        }
    }
}

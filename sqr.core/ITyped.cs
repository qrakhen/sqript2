using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public abstract class ITyped<T> where T : Enum
    {
        public T type { get; protected set; }

        public bool isType(T types)
        {
            return (((int)(object)types) & ((int)(object)type)) >= (int)(object)type;
        }
    }

    internal enum BitFlag
    {
        _1 = 1,
        _2 = 2,
        _3 = 4,
        _4 = 8,
        _5 = 16,
        _6 = 32,
        _7 = 64,
        _8 = 128,
        _9 = 256,
        _10 = 512,
        _11 = 1024,
        _12 = 2048,
        _13 = 4096,
        _14 = _13 * 2, // kb mehr xD
        _15 = _14 * 2,
        _16 = _15 * 2,
        _17 = _16 * 2,
        _18 = _17 * 2,
        _19 = _18 * 2,
        _20 = _19 * 2
    }
}

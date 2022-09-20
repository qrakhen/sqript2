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
            return type.Equals(types);
        }
    }
}

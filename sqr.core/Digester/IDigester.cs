using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{ 
    public abstract class Digester<TIn, TOut>
    {
        public virtual TOut digest(TIn input) { throw new SqrError("digest overload not implemented"); }
        public virtual TOut digest(TIn input, Qontext context) { throw new SqrError("digest overload not implemented"); }
        public virtual TOut digest(TIn input, Qontext context, object recursor) { throw new SqrError("digest overload not implemented"); }
    }
}

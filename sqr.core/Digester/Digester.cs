using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{ 
    public abstract class Digester<TIn, TOut>
    {
        protected readonly Logger log;

        public virtual TOut digest(TIn stack) { throw new SqrError("digest overload not implemented"); }
        public virtual TOut digest(TIn stack, Qontext context) { throw new SqrError("digest overload not implemented"); }
        public virtual TOut digest(TIn stack, Qontext context, object modifier) { throw new SqrError("digest overload not implemented"); }
        public virtual TOut digestUntil(TIn stack, Qontext context, string until) { throw new SqrError("digest overload not implemented"); }
    }
}

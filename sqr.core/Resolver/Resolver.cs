using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{ 
    public abstract class Resolver<TIn, TOut>
    {
        protected readonly Logger log;

        /*public virtual TOut resolve(TIn stack) { throw new SqrError("digest overload not implemented"); }
        public virtual TOut resolve(TIn stack, Qontext context) { throw new SqrError("digest overload not implemented"); }
        public virtual TOut resolve(TIn stack, Qontext context, object modifier) { throw new SqrError("digest overload not implemented"); }
        public virtual TOut resolveUntil(TIn stack, Qontext context, string until) { throw new SqrError("digest overload not implemented"); }*/
    }
}

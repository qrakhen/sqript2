using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class FunqtionResolver : Resolver<Stack<Token>, Funqtion>
    {
        public override Funqtion digest(Stack<Token> input)
        {
            log.spam("in " + GetType().Name);
            return null;
        }
    }
}

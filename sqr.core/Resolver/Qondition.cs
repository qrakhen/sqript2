using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class QonditionResolver : Resolver<Stack<Token>, Qondition>
    {
        public Qondition digest(Stack<Token> input)
        {
            log.spam("in " + GetType().Name);
            return null;
        }
    }
}

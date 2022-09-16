using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class QollectionDigester : Digester<Stack<Token>, Qollection>
    {
        public Qollection digest(Stack<Token> input)
        {
            log.spam("in " + GetType().Name);
            return null;
        }
    }
}

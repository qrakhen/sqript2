using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class QonditionDigester : Digester<Stack<Token>, Qondition>
    {
        public Qondition digest(Stack<Token> input)
        {
            return null;
        }
    }
}

using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    [Injectable]
    public class StructureDigester : Digester<Stack<Token>, Value>
    {
        public Value digest(Stack<Token> input, Qontext qontext)
        {
            return null;
        }
    }
}

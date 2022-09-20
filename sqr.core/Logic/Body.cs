using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Body
    {
        protected readonly Token[] content;

        public Body(Token[] content)
        {
            this.content = content;
        }

        public Stack<Token> getStack()
        {
            return new Stack<Token>(content);
        }
    }
}

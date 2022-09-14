using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class SqrError : Exception
    {
        public Value value;

        public SqrError(string message, Value value = null) : base(message)
        {
            this.value = value;
        }
    }
}

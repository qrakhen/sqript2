using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class SqrError : Exception
    {
        public object data;

        public SqrError(string message, object data = null) : base(message)
        {
            this.data = data;
        }
    }
}

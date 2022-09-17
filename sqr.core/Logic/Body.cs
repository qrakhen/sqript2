using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Body
    {
        public Qontext qontext { get; protected set; }
        public Token[] content { get; protected set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class SqrError : Exception
    {
        public static List<string> stackTrace = new List<string>();

        public object data;

        public SqrError(string message, object data = null) : base(message)
        {
            this.data = data;
        }
    }
    
    public class SqrNullError : SqrError
    {
        public SqrNullError(string message, object data = null) : base(message, data)
        {
            this.data = data;
        }
    }

    public class SqrParseError : SqrError
    {
        public Token token;

        public SqrParseError(string message, Token token = null, object data = null) : base(message, data)
        {
            this.token = token;
            this.data = token;
        }
    }

    public class SqrQontextError : SqrError
    {
        public Qontext qontext;

        public SqrQontextError(string message, Qontext qontext = null, object data = null) : base(message, data)
        {
            this.qontext = qontext;
        }
    }

    public class SqrModuleError : SqrError
    {
        public Module module;

        public SqrModuleError(string message, Module module = null, object data = null) : base(message, data)
        {
            this.module = module;
        }
    }

    public class SqrTypeError : SqrError
    {
        public SqrTypeError(string message, object data = null) : base(message, data)
        {
        }
    }
}

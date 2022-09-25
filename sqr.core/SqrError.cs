using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class SqrError : Exception
    {
        public static List<string> stackTrace = new List<string>();

        public object data;
        public object context;

        public SqrError(string message, object data = null, object context = null) : base(message)
        {
            this.data = data;
            this.context = context;
    }
    }
    
    public class SqrNullReferenceError : SqrError
    {
        public SqrNullReferenceError(string message, object data = null, object context = null) : base(message, data, context)
        {
        }
    }

    public class SqrParseError : SqrError
    {
        public new Token data;

        public SqrParseError(string message, Token token = null, object context = null) : base(message, token, context)
        {
            data = token;
        }
    }

    public class SqrQontextError : SqrError
    {
        public new Qontext data;

        public SqrQontextError(string message, Qontext qontext = null, object context = null) : base(message, qontext, context)
        {
            this.data = qontext;
        }
    }

    public class SqrModuleError : SqrError
    {
        public new Module data;

        public SqrModuleError(string message, Module module = null, object context = null) : base(message, module, context)
        {
            this.data = module;
        }
    }

    public class SqrTypeError : SqrError
    {
        public SqrTypeError(string message, object data = null, object context = null) : base(message, data, context)
        {
        }
    }
}

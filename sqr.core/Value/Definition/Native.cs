using Qrakhen.Sqr.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Qrakhen.Sqr.Core 
{
    public class NativeAttribute : Attribute
    {
        public string name;
        public Type type;
    }

    public class NativeMethod : NativeAttribute
    {
        public string[] parameters;
    }

    public class NativeField : NativeAttribute
    {
        
    }
}

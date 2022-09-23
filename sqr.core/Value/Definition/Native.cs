using System;

namespace Qrakhen.Sqr.Core 
{
    public class NativeAttribute : Attribute { }
    public class NativeMethodAttribute : NativeAttribute { }
    public class NativeFieldAttribute : NativeAttribute { }
    public class NativeGetterAttribute : NativeFieldAttribute { }
}

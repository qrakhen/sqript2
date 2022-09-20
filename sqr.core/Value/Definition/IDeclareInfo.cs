using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqr.Core
{
    public struct IDeclareInfo
    {
        public Type type;
        public Type.Access access;
        public bool isReference;
        public bool isFunqtion;
        public bool isReadonly;
        public string name;
    }
}

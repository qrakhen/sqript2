using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Qlass : Value<Type>
    {
        public Qlass(Type type) : base(type, Type.Qlass) 
        {
            
        }

        public override Value accessMember(string name)
        {
            if (__value == null)
                return Null;

            if (__value.methods.contains(name))
                return __value.methods[name].makeQallable(this);
            else
                throw new SqrTypeError("unknown member " + name + " of type " + __value.name);
        }

        public override string ToString()
        {
            return __value?.name;
        }

        public override string toDebugString()
        {
            return "Qlass [" + ToString() + "]";
        }
    }
}

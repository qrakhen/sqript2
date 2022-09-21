﻿using Newtonsoft.Json;
using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Qlass : Value
    {
        public Type declaringType;

        public Qlass() : base(Type.Qlass) { }
    }
}
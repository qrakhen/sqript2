﻿using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Instance : Value
    {
        public Instance(Type definition) : base(definition)
        {

        }
    }
}
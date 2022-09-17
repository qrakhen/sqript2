using Newtonsoft.Json;
using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    public class Objeqt : Value
    {
        private readonly Storage<string, Variable> properties = new Storage<string, Variable>();

        public Objeqt() : base(Type.Objeqt)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{ 
    public class Qonfig
    {
        public bool forceTypes = false;
        public bool typesCaseSensitive = true;
        public bool overwriteExistingNames = true;
        public bool useAliases = true;
        public bool useExtendedConsole = true;
        public Logger.Level loggingLevel = Logger.Level.CRITICAL;

    }
}

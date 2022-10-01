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
        public Logger.Level loggingLevel = Logger.Level.INFO;
        public Qonsole qonsole = new Qonsole();

        public class Qonsole
        {
            public bool useExtended = true;
            public Dictionary<Token.Type, string> colorMapping = new Dictionary<Token.Type, string>() {
                { Token.Type.Identifier, ConsoleColor.White.ToString() },
                { Token.Type.Boolean, ConsoleColor.DarkYellow.ToString() },
                { Token.Type.Operator, ConsoleColor.DarkRed.ToString() },
                { Token.Type.Number, ConsoleColor.Yellow.ToString() },
                { Token.Type.Type, ConsoleColor.Green.ToString() },
                { Token.Type.TypeValue, ConsoleColor.DarkGreen.ToString() },
                { Token.Type.Keyword, ConsoleColor.Blue.ToString() },
                { Token.Type.Accessor, ConsoleColor.DarkGray.ToString() },
                { Token.Type.String, ConsoleColor.Cyan.ToString() },
                { Token.Type.Structure, ConsoleColor.DarkGray.ToString() },
                { Token.Type.End, ConsoleColor.DarkGray.ToString() }
            };
        }
    }
}

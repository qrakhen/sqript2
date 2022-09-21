using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{
    internal class Validator
    {
        public class Token
        {
            public static bool isType(Core.Token token, Core.Token.Type type, bool throwError = false)
            {
                if (token == null) {
                    if (throwError)
                        throw new SqrParseError("expected a token, got nothing instead.");
                    return false;
                }
                if (!token.isType(type)) {
                    if (throwError)
                        throw new SqrParseError("expected token of type " + type + ", got " + token.type + " instead.", token);
                    return false;
                }
                return true;
            }

            public static bool isType<T>(Core.Token token, T type, bool throwError = false) where T : Enum
            {
                if (token == null) {
                    if (throwError)
                        throw new SqrParseError("expected a token, got nothing instead.");
                    return false;
                }
                var t = token.get<ITyped<T>>();
                if (t == null) {
                    if (throwError)
                        throw new SqrParseError("expected token of type " + typeof(T).Name + ", got " + token + " instead.", token);
                    return false;
                }
                if (!t.isType(type)) {
                    if (throwError)
                        throw new SqrParseError("expected token of type " + type.ToString() + ", got " + token + " instead.", token);
                    return false;
                }
                return true;
            }
        }
    }
}

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
                if (!token.hasType(type)) {
                    if (throwError)
                        throw new SqrParseError("expected token of type " + type + ", got " + token.type + " instead.", token);
                    return false;
                }
                return true;
            }

            public static bool tryGetType<T>(Core.Token token, Core.Token.Type type, out T value, bool throwError = false)
            {
                value = default(T);
                if (isType(token, type, throwError)) {
                    value = token.get<T>();
                    return true;
                } else
                    return false;
            }

            public static bool raw(Core.Token token, Core.Token.Type type, out string value, bool throwError = false)
            {
                value = null;
                if (isType(token, type, throwError)) {
                    value = token.raw;
                    return true;
                } else
                    return false;
            }

            public static bool isSubType<T, E>(Core.Token token, E type, bool throwError = false) where T : ITyped<E> where E : Enum
            {
                if (token == null) {
                    if (throwError)
                        throw new SqrParseError("expected a token, got nothing instead.");
                    return false;
                }
                var t = token.get<ITyped<E>>();
                if (t == null) {
                    if (throwError)
                        throw new SqrParseError("expected token of type " + typeof(T).Name + ", got " + token + " instead.", token);
                    return false;
                }
                if (!t.hasType(type)) {
                    if (throwError)
                        throw new SqrParseError("expected token of type " + type.ToString() + ", got " + token + " instead.", token);
                    return false;
                }
                return true;
            }

            public static bool tryGetSubType<T, E>(Core.Token token, E type, out T value, bool throwError = false) where T : ITyped<E> where E : Enum
            {
                value = default(T);
                if (isSubType<T, E>(token, type, throwError)) {
                    value = token.get<T>();
                    return true;
                } else
                    return false;
            }
        }
    }
}

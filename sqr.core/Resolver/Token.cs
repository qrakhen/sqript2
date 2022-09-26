using Qrakhen.SqrDI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Qrakhen.Sqr.Core
{  
    [Injectable]
    internal class TokenResolver : Resolver<Stack<char>, Stack<Token>>
    {
        private readonly Logger log;

        public Stack<Token> resolve(Stack<char> input, Qontext qontext = null)
        {
            log.verbose("in " + GetType().Name);
            var result = new List<Token>();
            long row = 0, count = 0, __prev = 0, __end;
            string value = null;
            Token.Type type = (Token.Type)0;
            Token token = null;
            while (!input.done) {
                try {
                    var pos = input.index;
                    if (input.peek() == '\n') {
                        row++;
                        __prev = pos;
                    }
                    if (input.peek() == '\0') {
                        input.digest();
                    }
                    type = matchType(input.peek());
                    value = readValue(type, input);
                    __end = input.index;
                    if (value != null) {
                        token = new Token(null, type, value);
                        token.__row = row;
                        token.__col = pos - __prev;
                        token.__pos = pos;
                        token.__end = __end;
                        // split for debugging reasons
                        token = Token.parse(token);
                        if (qontext != null && token.hasType(Token.Type.Identifier))
                            token.resolveType(qontext, false);
                        result.Add(token);
                    }
                } catch(SqrError e) {
                    throw new SqrParseError("error occured when parsing sqript @" + row + ":" + (input.index - __prev) + "(" + input.index + "): " + e.Message, token);
                }
            }
            log.spam(string.Join(", ", result.Select(_ => _.type + ": '" + _.raw + "'")));
            count = input.length;
            return new Stack<Token>(result.ToArray());
        }

        private string readString(Stack<char> input)
        {
            var start = input.digest();
            string buffer = "";

            do {
                buffer += new string(input.digestUntil(start));
                if (buffer.EndsWith('\\')) // escapes
                    buffer = buffer.Substring(0, buffer.Length - 1) + input.digest();
                else
                    break;
            } while (!input.done);

            if (input.digest() != start) // toss last ' or " and check if string is finished
                throw new SqrError("string without end detected! position: " + (input.index - buffer.Length) + ", content: " + buffer);

            return buffer;
        }

        private string readType(Stack<char> input, Token.Type type)
        {
            string buffer = "";
            while (matchType(input.peek()) == type) {
                buffer += input.digest();
                if (input.done)
                    break;
            }
            return buffer;
        }

        private string readIdentifier(Stack<char> input)
        {
            string buffer = "";
            while (
                    matchType(input.peek()) == Token.Type.Identifier ||
                    Regex.IsMatch(input.peek().ToString(), @"\d")) {
                buffer += input.digest();
                if (input.done)
                    break;
            }
            return buffer;
        }

        private string readValue(Token.Type type, Stack<char> input)
        {
            if (type == Token.Type.Comment) {
                input.digestUntil('\n');
                return null;
            }

            if (type == Token.Type.Whitespace) {
                input.digest();
                return null;
            }

            if (type == Token.Type.String)
                return readString(input);

            if (type == Token.Type.ValueOf) { 
                var r = input.digest() + readType(input, Token.Type.Identifier);
                return r;
            }

            if (type == Token.Type.Identifier)
                return readIdentifier(input);

            string buffer = "";
            while (type == matchType(input.peek())) {
                buffer += input.digest();
                if (input.done || type == Token.Type.Structure)
                    break;
            }

            return buffer;
        }

        private Token.Type matchType(char input)
        {
            foreach (var m in matches) {
                if (Regex.IsMatch(input.ToString(), m.Value))
                    return m.Key;
            }
            return Token.Type.Identifier;
        }

        static readonly private Dictionary<Token.Type, string> matches = new Dictionary<Token.Type, string>() {
            { Token.Type.Operator, @"[\/\-\*+=&<>^?!~:|]" },
            { Token.Type.Number, @"[\d.]" },
            { Token.Type.String, "[\"']" },
            { Token.Type.Structure, @"[{}()[\],]" },
            { Token.Type.End, @";" },
            //{ Token.Type.Accessor, @"[:]" },
            { Token.Type.ValueOf, "@" },
            { Token.Type.Whitespace, @"\s" },
            { Token.Type.Comment, @"#" },
        };
    }

    public class Token : ITyped<Token.Type>
    {
        public const string end = ";";

        public long __row, __col, __pos, __end;

        public readonly string raw;
        public object value;

        public Token(object value, Type type, string raw)
        {
            this.value = value;
            base.type = type;
            this.raw = raw;
        }

        public T get<T>()
        {
            if (!(value is T))
                return default(T);

            return (T)value;
        }

        public Value makeValue()
        {
            if (!isType(Type.Value))
                throw new SqrError("can not make value out of token: not a value token" + this, this);

            if (isType(Type.Boolean)) return new Boolean((bool)value);
            if (isType(Type.Float)) return new Number((float)value);
            if (isType(Type.Number)) return new Number((double)value);
            if (isType(Type.String)) return new String((string)value);
            if (isType(Type.String)) return new String((string)value);
            if (isType(Type.String)) return new String((string)value);
            if (isType(Type.Null)) return Value.Null;
            if (isType(Type.Void)) return Value.Void;
            throw new SqrError("no known native type applied to token " + this, this);
        }

        public Variable makeVariable(bool isReference = false, Core.Type strictType = null, bool isReadonly = false)
        {
            if (!isType(Type.Identifier))
                throw new SqrError("can not make variable out of token: not an identifier token" + this, this);

            return new Variable(null, isReference, strictType, isReadonly);
        }

        public NativeType asNativeType(Type type)
        {
            if (type == Type.Boolean) return NativeType.Boolean;
            if (type == Type.Float) return NativeType.Float;
            if (type == Type.Number) return NativeType.Number;
            if (type == Type.String) return NativeType.String;
            if (type == Type.Identifier) return NativeType.Variable;
            return NativeType.None;
        }

        public static Token parse(Token token)
        {
            Type parsedType;
            var value = __parse(token.raw, token.type, out parsedType);

            if (value == null)
                throw new SqrParseError("could not parse value " + token.raw + ", it's not a known " + token.type);

            token.value = value;
            token.type = parsedType;

            return token;
        }

        private static object __parse(string raw, Type type, out Type parsedType)
        {
            try {
                parsedType = type;
                if (raw == "null" || raw == "void") {
                    parsedType = (raw == "null" ? Type.Null : Type.Void);
                    return (raw == "null" ? Value.Null : Value.Void);
                }
                if (raw == "true" || raw == "false") {
                    parsedType = Type.Boolean;
                    return (raw == "true" ? true : false);
                }
                if (type == Type.Number) {
                    parsedType = Type.Number;
                    return double.Parse(raw, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
                if (type == Type.Operator) {
                    parsedType = Type.Operator;
                    var op = Operator.get(raw);
                    if (op == null) {
                        parsedType = Type.Keyword;
                        return Keyword.get(raw); // keyword aliases
                    } else
                        return op;
                }
                if (type == Type.Structure) {
                    parsedType = Type.Structure;
                    return Structure.get(raw); 
                }
                if (type == Type.ValueOf) {
                    object v = CoreModule.instance.getType(raw.Substring(1));
                    if (v != null) {
                        parsedType = Type.TypeValue;
                    } else {
                        parsedType = Type.IdentifierValue;
                        v = raw.Substring(1);
                    }
                    return v;
                }
                if (type == Type.Keyword || type == Type.Identifier) {
                    var v = Keyword.get(raw);
                    if (v != null) {
                        parsedType = Type.Keyword;
                        return v;
                    }                    
                }
                return raw;
            } catch (Exception e) {
                throw new SqrError("trying to parse raw token value " + raw + " as " + type + ". didn't work.");
            }
        }

        public Core.Type resolveType(Qontext qontext, bool doThrow = false)
        {
            Core.Type type = null;
            if (!Validator.Token.tryGetType(this, Type.Type, out type)) {
                if (Validator.Token.tryGetType(this, Type.Identifier, out string name)) {
                    type = qontext.resolveType(name, false);
                    if (type != null) {
                        this.type = Type.Type | (this.type & Type.ValueOf);
                        this.value = type;
                        Logger.TEMP_STATIC_DEBUG.verbose("could resolve type " + type + " for token " + this);
                    }
                }
            }

            if (type == null && doThrow) {
                throw new SqrTypeError("a value of Type or Qlass was expected, got " + this + " instead");
            }

            return type;
        }

        public override string ToString()
        {
            return type + " [ " + raw + " ] @" + __row + ":" + __col + ", p" + __pos;
        }

        [Flags]
        public enum Type
        {
            Operator = 1,
            Boolean = 2,
            Float = 4,
            Number = 8,
            String = 16,
            Structure = 32,
            Accessor = 64,
            Keyword = 128,
            Identifier = 256,
            Whitespace = 512,
            Comment = 1024,
            End = 2048,
            Type = 4096,
            Null = Type * 2,
            Void = Null * 2,
            ValueOf = Void * 2,

            Value = Boolean | Float | Number | String | Identifier | Null | Void | ValueOf,
            TypeValue = ValueOf | Type,
            IdentifierValue = ValueOf | Identifier
        }
    }
}

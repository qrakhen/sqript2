using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Qrakhen.Sqr.Core
{  
    [Injectable]
    public class TokenResolver : Resolver<Stack<char>, Stack<Token>>
    {
        private readonly Logger log;

        public override Stack<Token> digest(Stack<char> input)
        {
            log.spam("in " + GetType().Name);
            var result = new List<Token>();
            long col = 0, row = 0, count = 0, __prev = 0;
            while (!input.done) {
                if (input.peek() == '\n') {
                    row++;
                    col = 0;
                    __prev = input.index;
                }
                var type = matchType(input.peek());
                var value = readValue(type, input);
                if (value != null) {
                    var token = Token.create(value, type);
                    token.__row = row;
                    token.__col = input.index - __prev;
                    token.__pos = input.index;
                    result.Add(token);
                }
            }
            log.spam(string.Join(",", result.Select(_ => _.type + ": " + _.raw)));
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

            if (type == Token.Type.Type) {
                var r = input.digest() + readType(input, Token.Type.Identifier);
                if (input.peek() == '&')
                    return r + input.digest();
                else
                    return r;
            }

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
            { Token.Type.Operator, @"[\/\-\*+=&<>^?!~]" },
            { Token.Type.Number, @"[\d.]" },
            { Token.Type.String, "[\"']" },
            { Token.Type.Structure, @"[{}()[\],]" },
            { Token.Type.End, @";" },
            { Token.Type.Accessor, @"[.:]" },
            { Token.Type.Type, "@" },
            { Token.Type.Whitespace, @"\s" },
            { Token.Type.Comment, @"#" },
        };
    }

    public class Token : ITyped<Token.Type>
    {
        public long __row, __col, __pos = -1;

        public readonly string raw;
        public readonly object value;
        public new Type type => base.type;

        private Token(object value, Type type, string raw)
        {
            this.value = value;
            base.type = type;
            this.raw = raw;
        }

        public T get<T>()
        {
            return (T)value;
        }

        public Value makeValue()
        {
            if (!isType(Type.Value))
                throw new SqrError("can not make value out of token: not a value token" + this);

            if (type == Type.Boolean) return new Boolean((bool)value);
            if (type == Type.Float) return new Number((float)value);
            if (type == Type.Number) return new Number((double)value);
            if (type == Type.String) return new String((string)value);
            throw new SqrError("no known native type applied to token " + this);
        }

        public Variable makeVariable(bool isReference = false, bool isStrictType = false, bool isReadonly = false)
        {
            if (!isType(Type.Identifier))
                throw new SqrError("can not make variable out of token: not an identifier token" + this);

            return new Variable(null, isReference, isStrictType, isReadonly);
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

        public static Token create(string raw, Type type)
        {
            var value = parse(raw, type);

            if (value == null)
                throw new SqrError("could not parse value " + raw + ", it's not a known " + type);

            if (value.GetType() == typeof(Boolean))
                type = Type.Boolean;

            if (value.GetType() == typeof(Keyword))
                type = Type.Keyword;

            return new Token(value, type, raw);
        }

        public static object parse(string raw, Type type)
        {
            try {
                if (raw == "true" || raw == "false") return (raw == "true" ? true : false);
                if (type == Type.Number) return double.Parse(raw, System.Globalization.NumberFormatInfo.InvariantInfo);
                if (type == Type.Operator) return Operator.get(raw);
                if (type == Type.Structure) return Structure.get(raw);
                if (type == Type.Keyword || type == Type.Identifier) {
                    var v = Keyword.get(raw);
                    if (v != null) return v;
                }
                return raw;
            } catch (Exception e) {
                throw new SqrError("trying to parse raw token value " + raw + " as " + type + ". didn't work.");
            }
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

            Value = Boolean | Float | Number | String | Identifier
        }
    }
}

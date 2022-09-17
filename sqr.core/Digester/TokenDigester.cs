using Qrakhen.Dependor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Qrakhen.Sqr.Core
{  
    [Injectable]
    public class TokenDigester : Digester<Stack<char>, Stack<Token>>
    {
        private readonly Logger log;

        public Stack<Token> digest(Stack<char> input)
        {
            log.spam("in " + GetType().Name);
            var result = new List<Token>();
            while (!input.done) {
                var type = matchType(input.peek());
                var value = readValue(type, input);
                if (value != null)
                    result.Add(Token.create(value, type));
            }
            log.spam(string.Join(",", result.Select(_ => _.type + ": " + _.raw)));
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
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{  
    public class Token : ITyped<Token.Type>
    {
        public readonly object value;
        public new Type type => base.type;

        private Token(object value, Type type)
        {
            this.value = value;
            base.type = type;
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
            if (type == Type.Number) return new Number((double)value);
            if (type == Type.String) return new String((string)value);
            return new Value(Value.Type.None, true);
        }

        public Variable makeVariable(bool isReference = false, bool isStrictType = false, bool isReadonly = false)
        {
            if (!isType(Type.Identifier))
                throw new SqrError("can not make variable out of token: not an identifier token" + this);

            return new Variable(null, isReference, isStrictType, isReadonly);
        }

        public Value.Type toValueType(Type type)
        {
            if (type == Type.Boolean) return Value.Type.Boolean;
            if (type == Type.Number) return Value.Type.Number;
            if (type == Type.String) return Value.Type.String;
            if (type == Type.Identifier) return Value.Type.Variable;
            return Value.Type.None;
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

            return new Token(value, type);
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
            } catch(Exception e) {
                throw new SqrError("trying to parse raw token value " + raw + " as " + type + ". didn't work.");
            }
        }

        public override string ToString()
        {
            return type + "[ " + value + " ]";
        }

        [Flags]
        public enum Type
        {
            Operator = 1,
            Boolean = 2,
            Number = 4,
            String = 8,
            Structure = 16,
            Accessor = 32,
            Keyword = 64,
            Identifier = 128,
            Whitespace = 256,
            Comment = 512,
            End = 1024,

            Value = Boolean | Number | String | Identifier
        }       
    }
}

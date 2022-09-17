using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqr.Core
{  
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
            } catch(Exception e) {
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

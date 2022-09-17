using System;
using System.Collections.Generic;
using System.Linq;

namespace Qrakhen.Sqr.Core
{
    public delegate Value ExtenderFunqtion(Value[] parameters, Value self);
    public class ExtenderFunqtionAttribute : Attribute { }

    public class Value : ITyped<Value.Type>
    {
        public static Value Null => new Value(Type.Null, false);

        private static readonly Storage<System.Type, Storage<string, ExtenderFunqtion>> extensions =
            new Storage<System.Type, Storage<string, ExtenderFunqtion>>();
        private readonly Storage<string, Member> members = new Storage<string, Member>();
        public readonly bool isPrimitive;

        public Value(Value.Type type = Type.None, bool isPrimitive = false)
        {
            this.type = type;
            this.isPrimitive = isPrimitive;
        }

        [ExtenderFunqtion] // vielleicht doch lieber mit echten funqtions?
        public static Value toString(Value[] parameters, Value self)
        {
            return new String(self.ToString());
        }

        public override bool Equals(object obj)
        {
            if (type == Type.Null && obj is Value)
                return (obj as Value).type == type;

            return base.Equals(obj);
        }

        static Value()
        {
            var types = new System.Type[] {
                typeof(Value),
                typeof(String),
                typeof(Number),
                typeof(Boolean),
                typeof(Objeqt),
                typeof(Qollection),
                typeof(Funqtion)
            };

            foreach (var type in types) {
                extensions[type] = new Storage<string, ExtenderFunqtion>();

                type
                    .GetMethods()
                    .Where(_ => Attribute.GetCustomAttribute(_, typeof(ExtenderFunqtionAttribute)) != null)
                    .ToList()
                    .ForEach(_ => {
                        Dependor.Dependor.get<Logger>().debug("loading native extension function " + type.Name + ":" + _.Name);
                        extensions[typeof(Value)][_.Name] = (parameters, self) => { return (Value)_.Invoke(self, parameters); };
                    });
            }
        }

        [Flags]
        public enum Type
        {
            None = default,
            Boolean = 1,
            Number = 2,
            String = 4,
            Qollection = 8,
            Objeqt = 16,
            Funqtion = 32,
            Qontext = Qollection | Objeqt | Funqtion,
            Variable = 64,
            Null = 128
        }
    }

    public class Value<T> : Value
    {
        protected T __value;

        public Value(T value = default(T), Value.Type type = Type.None, bool isPrimitive = false) : base(type, isPrimitive)
        {
            __value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Value<T>)
                return (obj as Value<T>).__value.Equals(__value);

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return __value.ToString();
        }
    }
}

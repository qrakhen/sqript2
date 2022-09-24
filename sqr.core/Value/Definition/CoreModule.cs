using System.Collections.Generic;
using static Qrakhen.Sqr.Core.Type;

namespace Qrakhen.Sqr.Core 
{
    public class CoreModule : Module
    {
        internal static Module sqriptModule { get; private set; }
        internal static Module instance { get; private set; }

        public CoreModule() : base("Core")
        {
        }

        static CoreModule()
        {
            sqriptModule = new Module("Sqript");
            instance = new Module("Core");
            sqriptModule.appendModule(instance);

            var types = new List<Type>();

            var value = Type.create(typeof(Value), new Args {
                name = "Value",
                nativeType = NativeType.None,
            });

            types.Add(value);

            types.Add(Type.create(typeof(String), new Args {
                name = "String",
                nativeType = NativeType.String,
                extends = value,
            }));

            types.Add(Type.create(typeof(Number), new Args {
                name = "Number",
                nativeType = NativeType.Number,
                extends = value,
            }));

            types.Add(Type.create(typeof(Integer), new Args {
                name = "Integer",
                nativeType = NativeType.Integer,
                extends = value,
            }));

            types.Add(Type.create(typeof(Boolean), new Args {
                name = "Boolean",
                nativeType = NativeType.Boolean,
                extends = value,
            }));

            types.Add(Type.create(typeof(Qollection), new Args {
                name = "Qollection",
                nativeType = NativeType.Qollection,
                extends = value,
            }));

            types.Add(Type.create(typeof(Objeqt), new Args {
                name = "Objeqt",
                nativeType = NativeType.Objeqt,
                extends = value,
            }));

            types.Add(Type.create(typeof(Array), new Args {
                name = "Array",
                nativeType = NativeType.Array,
                extends = value,
            }));

            types.Add(Type.create(typeof(Qallable), new Args {
                name = "Qallable",
                nativeType = NativeType.Funqtion,
                extends = value,
            }));

            types.Add(Type.create(typeof(Variable), new Args {
                name = "Variable",
                nativeType = NativeType.Variable,
                extends = value,
            }));

            types.Add(Type.create(typeof(Module), new Args {
                name = "Module",
                nativeType = NativeType.Module,
                extends = value
            }));

            types.Add(Type.create(typeof(Qonsole), new Type.Args {
                name = "Qonsole",
                extends = Type.Value,
                nativeType = NativeType.Static
            }));

            types.Add(Type.create(typeof(Random), new Type.Args {
                name = "Random",
                extends = Type.Value,
                nativeType = NativeType.Instance
            }));

            types.Add(Type.create(typeof(Time), new Type.Args {
                name = "Time",
                extends = Type.Value,
                nativeType = NativeType.Instance
            }));

            types.Add(Type.create(typeof(Parse), new Type.Args {
                name = "Parse",
                extends = Type.Value,
                nativeType = NativeType.Static
            }));

            types.Add(Type.create(typeof(Calc), new Type.Args {
                name = "Calc",
                extends = Type.Value,
                nativeType = NativeType.Static
            }));

            types.ForEach(_ => instance.export(new Qlass(_)));
        }
    }
}

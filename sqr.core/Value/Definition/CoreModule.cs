using System.Collections.Generic;
using static Qrakhen.Sqr.Core.Type;

namespace Qrakhen.Sqr.Core 
{
    public class CoreModule : Module
    {
        internal static Module sqriptModule { get; private set; }
        internal static Module instance { get; private set; }

        internal CoreModule() : base("Core")
        {
        }

        internal static void init()
        {
            sqriptModule = new Module("Sqript");
            instance = new Module("Core");
            sqriptModule.appendModule(instance);

            var types = new List<Type>();

            var value = Type.create(typeof(Value), new Args {
                name = "Value",
                nativeType = NativeType.None,
            });

            instance.export(value);

            instance.export(Type.create(typeof(String), new Args {
                name = "String",
                nativeType = NativeType.String,
                extends = value,
            }));

            instance.export(Type.create(typeof(Number), new Args {
                name = "Number",
                nativeType = NativeType.Number,
                extends = value,
            }));

            instance.export(Type.create(typeof(Integer), new Args {
                name = "Integer",
                nativeType = NativeType.Integer,
                extends = value,
            }));

            instance.export(Type.create(typeof(Boolean), new Args {
                name = "Boolean",
                nativeType = NativeType.Boolean,
                extends = value,
            }));

            instance.export(Type.create(typeof(Qollection), new Args {
                name = "Qollection",
                nativeType = NativeType.Qollection,
                extends = value,
            }));

            instance.export(Type.create(typeof(Objeqt), new Args {
                name = "Objeqt",
                nativeType = NativeType.Objeqt,
                extends = value,
            }));

            instance.export(Type.create(typeof(Array), new Args {
                name = "Array",
                nativeType = NativeType.Array,
                extends = value,
            }));

            instance.export(Type.create(typeof(Qallable), new Args {
                name = "Qallable",
                nativeType = NativeType.Funqtion,
                extends = value,
            }));

            instance.export(Type.create(typeof(Variable), new Args {
                name = "Variable",
                nativeType = NativeType.Variable,
                extends = value,
            }));

            instance.export(Type.create(typeof(Qonsole), new Type.Args {
                name = "Qonsole",
                extends = value,
                nativeType = NativeType.Static
            }));

            instance.export(Type.create(typeof(Random), new Type.Args {
                name = "Random",
                extends = value,
                nativeType = NativeType.Instance
            }));

            instance.export(Type.create(typeof(Time), new Type.Args {
                name = "Time",
                extends = value,
                nativeType = NativeType.Instance
            }));

            instance.export(Type.create(typeof(Parse), new Type.Args {
                name = "Parse",
                extends = value,
                nativeType = NativeType.Static
            }));

            instance.export(Type.create(typeof(Calc), new Type.Args {
                name = "Calc",
                extends = value,
                nativeType = NativeType.Static
            }));
        }
    }
}

namespace Qrakhen.Sqr.Core 
{
    public class Module : Value
    {
        internal static Type type = 
            Type.create(typeof(Module), new Type.Args {
                name = "Module",
                nativeType = NativeType.Module
            });

        public readonly string name;
        public readonly Storage<string, Module> children = new Storage<string, Module>();
        public readonly Storage<string, Value> exports = new Storage<string, Value>();

        public Module(string name) : base(type)
        {
            this.name = name;
        }

        public Value get(string name)
        {
            return exports[name];
        }

        public Type getType(string name)
        {
            var v = exports[name];
            if (v != null) {
                if (v is Qlass)
                    return (v as Qlass).raw_t;
            }
            return null;
        }

        public void appendModule(Module module)
        {
            if (children.contains(module.name))
                return;
            
            children[module.name] = module;
        }

        public override Value accessMember(Value name)
        {
            var key = name as String;
            if (children.contains(key))
                return children[key];
            else if (exports.contains(key))
                return exports[key];
            else
                throw new SqrModuleError("could not find name " + name + " in module " + this.name);
        }

        public void export(Value value, string asName = null)
        {
            var name = asName ?? value.type.name;
            if (exports.contains(name))
                throw new SqrModuleError("can not export " + value + " as " + name + ": exports already contain something by that name.");

            exports[name] = value;
            Logger.TEMP_STATIC_DEBUG.debug("exported " + value + " as " + name);
        }

        public void export(Type type) => export(new Qlass(type), type.name);
    }
}

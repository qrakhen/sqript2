namespace Qrakhen.Sqr.Core 
{
    public class Module
    {
        public readonly string name;
        public readonly Module parent;
        public readonly Storage<string, Value> exports = new Storage<string, Value>();

        public Module(string name, Module parent)
        {
            this.name = name;
            this.parent = parent;
        }

        public void export(Value value, string asName = null)
        {

        }
    }
}

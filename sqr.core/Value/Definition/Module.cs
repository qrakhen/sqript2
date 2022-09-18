namespace Qrakhen.Sqr.Core 
{
    public class Module
    {
        public readonly string name;
        public readonly Module parent;
        public Module(string name, Module parent)
        {
            this.name = name;
            this.parent = parent;
        }
    }
}

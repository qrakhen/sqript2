namespace Qrakhen.Sqr.Core 
{
    public class Module
    {
        public readonly string name;
        public readonly Module parent;
        public Qontext qontext;

        public Module(string name, Module parent, Qontext parentQontext = null)
        {
            this.name = name;
            this.parent = parent;
            this.qontext = new Qontext(parentQontext);
        }
    }
}

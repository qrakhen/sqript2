using Qrakhen.Sqr.Core;

namespace Qrakhen.Sqr.Core 
{ 
    public class ValueDefinition : TypeDefinition
    {
        public static Value toString(Value self)
        {
            return new String(self.ToString());
        }

        static ValueDefinition()
        {
            // just as an idea...
            register(new Args
            {
                name = "Value",
                module = "Sqr",
                nativeType = NativeType.None,
                fields = null,
                methods = new Storage<string, Method>()
                {
                    { "toString", new Method(
                        Access.Public, 
                        new InternalFunqtion((p, self) => toString(self))) }
                }
            });
        }
    }
}

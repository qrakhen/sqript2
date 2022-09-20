namespace Qrakhen.Sqr.Core.Test
{
    public class Test
    {
        public void assertEqual(object a, object b)
        {
        }

        public void assertEqual(Value a, Value b)
        {
            if (a.raw != b.raw)
                throw new System.Exception("aaa");
        }
    }
}
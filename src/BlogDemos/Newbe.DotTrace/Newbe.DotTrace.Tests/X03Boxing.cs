using NUnit.Framework;

namespace Newbe.DotTrace.Tests
{
    public class X03Boxing
    {
        [Test]
        public void Boxing()
        {
            for (int i = 0; i < 1_000_000; i++)
            {
                UseObject(i);
            }
        }

        [Test]
        public void NoBoxing()
        {
            for (int i = 0; i < 1_000_000; i++)
            {
                UseInt(i);
            }
        }

        public static void UseInt(int age)
        {
            // nothing
        }

        public static void UseObject(object obj)
        {
            // nothing
        }
    }
}
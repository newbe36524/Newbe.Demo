using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Newbe.DotTrace.Tests
{
    public class X01StringBuilderTest
    {
        [Test]
        public void UsingString()
        {
            var source = Enumerable.Range(0, 10)
                .Select(x => x.ToString())
                .ToArray();
            var re = string.Empty;
            for (int i = 0; i < 10_000; i++)
            {
                re += source[i % 10];
            }
        }

        [Test]
        public void UsingStringBuilder()
        {
            var source = Enumerable.Range(0, 10)
                .Select(x => x.ToString())
                .ToArray();
            var sb = new StringBuilder();
            for (var i = 0; i < 10_000; i++)
            {
                sb.Append(source[i % 10]);
            }

            var _ = sb.ToString();
        }
    }
}
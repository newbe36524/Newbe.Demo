namespace Try_More_On_IEnumerable
{
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/bMtGG3
    /// </summary>
    public class T04Linq产生数值
    {
        public static void Run()
        {
            // 系统内置了一个方法来获取数字的自增序列，因此可以如此简化
            var result = Enumerable
                .Range(0, 11)
                .Where(x => x % 2 == 0);

            result.Should().Equal(0, 2, 4, 6, 8, 10);
        }
    }
}
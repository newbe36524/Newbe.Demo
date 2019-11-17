namespace Try_More_On_IEnumerable
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/fuPPbW
    /// </summary>
    public class T05输出233
    {
        public static void Run()
        {
            // 输出3个数字 233
            var result = Get233().Take(3);
            result.Should().Equal(2, 3, 3);

            // 输出5个数字 23333
            result = Get233().Take(5);
            result.Should().Equal(2, 3, 3, 3, 3);

            IEnumerable<int> Get233()
            {
                // 第一次输出 2
                yield return 2;
                while (true)
                {
                    // 后面都输出 3
                    yield return 3;
                }
            }

            /**
             * 值得注意的是 while(true) 并不会导致程序陷入死循环
             * 因为 yield return 是采用按需供给的方式执行的。
             * 关于这点可以参考右侧文章：https://www.cnblogs.com/SilentCode/p/5014068.html
             */
        }
    }
}
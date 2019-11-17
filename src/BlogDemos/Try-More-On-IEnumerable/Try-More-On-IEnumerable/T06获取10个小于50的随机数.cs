namespace Try_More_On_IEnumerable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/iqFoq3
    /// </summary>
    public class T06获取10个小于50的随机数
    {
        public static void Run()
        {
            /**
          * 过滤随机数中小于50的数字，只获取其中10个结果
          */
            var result = GetRandom()
                .Where(x => x < 50)
                .Take(10)
                .ToArray();

            // 断言
            result.Should().HaveCount(10);
            result.All(x => x < 50).Should().BeTrue();

            IEnumerable<int> GetRandom()
            {
                var random = new Random();
                // 不断输出 0-100 的随机数
                while (true)
                {
                    yield return random.Next(0, 100);
                }
            }
        }
    }
}
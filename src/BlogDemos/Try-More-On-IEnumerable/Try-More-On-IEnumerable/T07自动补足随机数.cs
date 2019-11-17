namespace Try_More_On_IEnumerable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/HG0kd6
    /// </summary>
    public class T07自动补足随机数
    {
        public static void Run()
        {
            // 获取3个数字
            var result1 = GetData().Take(3);

            // 3 个数字分别为 0,1,2
            result1.Should().Equal(0, 1, 2);

            // 获取 10 个数字
            var result2 = GetData().Take(10).ToArray();

            // 前 5 个数字分别为 0,1,2,3,4
            result2.Take(5).Should().Equal(0, 1, 2, 3, 4);
            // 从第 5 个开始的数字都大于 10
            result2.Skip(5).Take(5).Should().Match(x => x.All(a => a > 10));

            /**
             * 获取一组数据，前5个数字是 0,1,2,3,4 。后面继续获取则为随机数
             */
            IEnumerable<int> GetData()
            {
                var staticData = new[] {0, 1, 2, 3, 4};
                foreach (var i in staticData)
                {
                    yield return i;
                }

                var random = new Random();
                // 不断输出 10-100 的随机数
                while (true)
                {
                    yield return random.Next(10, 100);
                }
            }
        }
    }
}
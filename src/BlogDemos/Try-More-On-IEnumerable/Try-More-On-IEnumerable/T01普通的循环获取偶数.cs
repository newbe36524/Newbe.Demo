// @nuget: FluentAssertions

namespace Try_More_On_IEnumerable
{
    using System.Collections.Generic;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/elXUjA
    /// </summary>
    public class T01普通的循环获取偶数
    {
        public static void Run()
        {
            // 创建一个集合存放最终结果
            var result = new List<int>();
            for (var i = 0; i <= 10; i++)
            {
                // 如果是偶数的话，将数字放入集合中
                if (i % 2 == 0)
                {
                    result.Add(i);
                }
            }

            // 断言最终的结果
            result.Should().Equal(0, 2, 4, 6, 8, 10);
        }
    }
}
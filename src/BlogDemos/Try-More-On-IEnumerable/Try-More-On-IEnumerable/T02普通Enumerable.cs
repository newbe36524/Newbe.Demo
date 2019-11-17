namespace Try_More_On_IEnumerable
{
    using System.Collections.Generic;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/Qp10IH
    /// </summary>
    public class T02普通Enumerable
    {
        public static void Run()
        {
            // 通过静态方法获取偶数的Enumerable对象
            var result = GetNumber(10);

            // 断言结果
            result.Should().Equal(0, 2, 4, 6, 8, 10);

            /**
             * 这种写法比起 T01 的写法少了一个集合的创建，通常来说是性能更佳的写法
             */
        }

        /// <summary>
        /// 获取小于等于max的偶数
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        private static IEnumerable<int> GetNumber(int max)
        {
            for (var i = 0; i <= max; i++)
            {
                if (i % 2 == 0)
                {
                    // 使用 yield return 返回每次迭代的结果
                    yield return i;
                }
            }
        }
    }
}
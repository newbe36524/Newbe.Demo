namespace Try_More_On_IEnumerable
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/c7NMNI
    /// </summary>
    public class T03分离条件
    {
        public static void Run()
        {
            /**
          * 使用 Where 过滤结果
          * 这样的做法分离了“自增”逻辑和“判断是偶数”的逻辑
          */
            var result = GetNumber(10)
                .Where(x => x % 2 == 0);

            result.Should().Equal(0, 2, 4, 6, 8, 10);

            // 迭代时只是进行了自增操作
            IEnumerable<int> GetNumber(int max)
            {
                for (var i = 0; i <= max; i++)
                {
                    yield return i;
                }
            }
        }
    }
}
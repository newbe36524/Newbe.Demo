namespace Try_More_On_IEnumerable
{
    using System.Collections.Generic;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/MQ1TSg
    /// </summary>
    public class T02本地函数
    {
        public static void Run()
        {
            // 通过本地函数获取结果
            var result = GetNumber(10);

            // 断言结果
            result.Should().Equal(0, 2, 4, 6, 8, 10);

            /**
             * 使用本地函数获取 Enumerable 对象。
             * 此处的效果和静态方法类似。
             * 本地函数是 C# 7.0 才开始支持的语法特性。
             * 关于本地函数详细内容可以参见：https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/local-functions
             */
            IEnumerable<int> GetNumber(int max)
            {
                for (var i = 0; i <= max; i++)
                {
                    if (i % 2 == 0)
                    {
                        yield return i;
                    }
                }
            }
        }
    }
}
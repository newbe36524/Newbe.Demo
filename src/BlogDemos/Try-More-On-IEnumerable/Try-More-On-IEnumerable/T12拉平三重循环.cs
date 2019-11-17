namespace Try_More_On_IEnumerable
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/SgRu1z
    /// </summary>
    public class T12拉平三重循环
    {
        public static void Run()
        {
            /**
           * 通过本地函数获取 0-999 共 1000 个数字。
           * 在 GetSomeData 通过三重循环构造这些数据
           * 值得注意的是 GetSomeData 隐藏了三重循环的细节
           */
            var result1 = GetSomeData(10, 10, 10)
                .ToArray();

            /**
             * 与 GetSomeData 方法对比，将“遍历”和“处理”两个逻辑进行了分离。
             * “遍历”指的是三重循环本身。
             * “处理”指的是三重循环最内部的加法过程。
             * 这里通过 Select 方法，将“处理”过程抽离了出来。
             * 这其实和 “T03分离条件”中使用 Where 使用的是相同的思想。
             */
            var result2 = GetSomeData2(10, 10, 10)
                .Select(tuple => tuple.i * 100 + tuple.j * 10 + tuple.k)
                .ToArray();

            // 生成一个 0-999 的数组。
            var result = Enumerable.Range(0, 1000).ToArray();

            result1.Should().Equal(result);
            result2.Should().Equal(result);

            IEnumerable<int> GetSomeData(int maxI, int maxJ, int maxK)
            {
                for (var i = 0; i < maxI; i++)
                {
                    for (var j = 0; j < maxJ; j++)
                    {
                        for (var k = 0; k < maxK; k++)
                        {
                            yield return i * 100 + j * 10 + k;
                        }
                    }
                }
            }

            IEnumerable<(int i, int j, int k)> GetSomeData2(int maxI, int maxJ, int maxK)
            {
                for (var i = 0; i < maxI; i++)
                {
                    for (var j = 0; j < maxJ; j++)
                    {
                        for (var k = 0; k < maxK; k++)
                        {
                            yield return (i, j, k);
                        }
                    }
                }
            }
        }
    }
}
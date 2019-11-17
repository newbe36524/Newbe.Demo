namespace Try_More_On_IEnumerable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/dK8l6Y
    /// </summary>
    public class T15分页
    {
        public static void Run()
        {
            var arraySource = new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

            // 使用迭代器进行分页，每 3 个一页
            var enumerablePagedResult = PageByEnumerable(arraySource, 3).ToArray();

            // 结果一共 4 页
            enumerablePagedResult.Should().HaveCount(4);
            // 最后一页只有一个数字，为 9 
            enumerablePagedResult.Last().Should().Equal(9);


            // 通过常规的 Skip 和 Take 来分页是最为常见的办法。结果应该与上面的分页结果一样
            var result3 = NormalPage(arraySource, 3).ToArray();

            result3.Should().HaveCount(4);
            result3.Last().Should().Equal(9);

            IEnumerable<IEnumerable<int>> PageByEnumerable(IEnumerable<int> source, int pageSize)
            {
                var onePage = new LinkedList<int>();
                foreach (var i in source)
                {
                    onePage.AddLast(i);
                    if (onePage.Count != pageSize)
                    {
                        continue;
                    }

                    yield return onePage;
                    onePage = new LinkedList<int>();
                }

                // 最后一页如果数据不足一页，也应该返回该页
                if (onePage.Count > 0)
                {
                    yield return onePage;
                }
            }

            IEnumerable<IEnumerable<int>> NormalPage(IReadOnlyCollection<int> source, int pageSize)
            {
                var pageCount = Math.Ceiling(1.0 * source.Count / pageSize);
                for (var i = 0; i < pageCount; i++)
                {
                    var offset = i * pageSize;
                    var onePage = source
                        .Skip(offset)
                        .Take(pageSize);
                    yield return onePage;
                }
            }

            /**
             * 从写法逻辑上来看，显然 NormalPage 的写法更容易让大众接受
             * PageByEnumerable 写法在仅仅只有在一些特殊的情况下才能体现性能上的优势，可读性上却不如 NormalPage
             */
        }
    }
}
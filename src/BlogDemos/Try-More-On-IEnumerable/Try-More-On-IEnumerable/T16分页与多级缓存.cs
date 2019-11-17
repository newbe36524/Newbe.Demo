namespace Try_More_On_IEnumerable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/RPWoSA
    /// </summary>
    public class T16分页与多级缓存
    {
        public static void Run()
        {
            /**
            * 获取 5 页数据，每页 2 个。
            * 依次从 内存、Redis、ElasticSearch和数据库中获取数据。
            * 先从内存中获取数据，如果内存中数据不足页，则从 Redis 中获取。
            * 若 Redis 获取后还是不足页，进而从 ElasticSearch 中获取。依次类推，直到足页或者再无数据
            */
            const int pageSize = 2;
            const int pageCount = 5;
            var emptyData = Enumerable.Empty<int>().ToArray();

            /**
             * 初始化各数据源的数据，除了内存有数据外，其他数据源均没有数据
             */
            var memoryData = new[] {0, 1, 2};
            var redisData = emptyData;
            var elasticSearchData = emptyData;
            var databaseData = emptyData;

            var result = GetSourceData()
                // ToPagination 是一个扩展方法。此处是为了体现链式调用的可读性，转而使用扩展方法，没有使用本地函数
                .ToPagination(pageCount, pageSize)
                .ToArray();

            result.Should().HaveCount(2);
            result[0].Should().Equal(0, 1);
            result[1].Should().Equal(2);

            /**
             * 初始化各数据源数据，各个数据源均有一些数据
             */
            memoryData = new[] {0, 1, 2};
            redisData = new[] {3, 4, 5};
            elasticSearchData = new[] {6, 7, 8};
            databaseData = Enumerable.Range(9, 100).ToArray();

            var result2 = GetSourceData()
                .ToPagination(pageCount, pageSize)
                .ToArray();

            result2.Should().HaveCount(5);
            result2[0].Should().Equal(0, 1);
            result2[1].Should().Equal(2, 3);
            result2[2].Should().Equal(4, 5);
            result2[3].Should().Equal(6, 7);
            result2[4].Should().Equal(8, 9);

            IEnumerable<int> GetSourceData()
            {
                // 将多数据源的数据连接在一起
                var data = GetDataSource()
                    .SelectMany(x => x);
                return data;

                // 获取数据源
                IEnumerable<IEnumerable<int>> GetDataSource()
                {
                    // 将数据源依次返回
                    yield return GetFromMemory();
                    yield return GetFromRedis();
                    yield return GetFromElasticSearch();
                    yield return GetFromDatabase();
                }

                IEnumerable<int> GetFromMemory()
                {
                    Console.WriteLine("正在从内存中获取数据");
                    return memoryData;
                }

                IEnumerable<int> GetFromRedis()
                {
                    Console.WriteLine("正在从Redis中获取数据");
                    return redisData;
                }

                IEnumerable<int> GetFromElasticSearch()
                {
                    Console.WriteLine("正在从ElasticSearch中获取数据");
                    return elasticSearchData;
                }

                IEnumerable<int> GetFromDatabase()
                {
                    Console.WriteLine("正在从数据库中获取数据");
                    return databaseData;
                }
            }

            /**
             * 值得注意的是：
             * 由于 Enumerable 按需迭代的特性，如果将 result2 的所属页数改为只获取 1 页。
             * 则在执行数据获取时，将不会再控制台中输出从 Redis、ElasticSearch和数据库中获取数据。
             * 也就是说，并没有执行这些操作。读者可以自行修改以上代码，加深印象。
             */
        }
    }

    public static class EnumerableExtensions
    {
        /// <summary>
        /// 将原数据分页
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="pageCount">页数</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> ToPagination(this IEnumerable<int> source,
            int pageCount,
            int pageSize)
        {
            var maxCount = pageCount * pageSize;
            var countNow = 0;
            var onePage = new LinkedList<int>();
            foreach (var i in source)
            {
                onePage.AddLast(i);
                countNow++;

                // 如果获取的数量已经达到了分页所需要的总数，则停止进一步迭代
                if (countNow == maxCount)
                {
                    break;
                }

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
    }
}
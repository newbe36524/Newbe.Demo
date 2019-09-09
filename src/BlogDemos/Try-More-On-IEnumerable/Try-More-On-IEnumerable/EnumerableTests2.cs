using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Try_More_On_IEnumerable
{
    public class EnumerableTests2
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EnumerableTests2(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void T11分组合并()
        {
            var array1 = new[] {0, 1, 2, 3, 4};
            var array2 = new[] {5, 6, 7, 8, 9};

            // 通过本地方法合并两个数组为一个数据
            var result1 = ConcatArray(array1, array2).ToArray();

            // 使用 Linq 中的 Concat 来合并两个 IEnumerable 对象
            var result2 = array1.Concat(array2).ToArray();

            // 使用 Linq 中的 SelectMany 将 “二维数据” 拉平合并为一个数组
            var result3 = new[] {array1, array2}.SelectMany(x => x).ToArray();

            /**
             *  使用 Enumerable.Range 生成一个数组，这个数据的结果为
             *  0,1,2,3,4,5,6,7,8,9
             */
            var result = Enumerable.Range(0, 10).ToArray();

            // 通过以上三种方式合并的结果时相同的
            result1.Should().Equal(result);
            result2.Should().Equal(result);
            result3.Should().Equal(result);

            IEnumerable<T> ConcatArray<T>(IEnumerable<T> source1, IEnumerable<T> source2)
            {
                foreach (var item in source1)
                {
                    yield return item;
                }

                foreach (var item in source2)
                {
                    yield return item;
                }
            }
        }

        [Fact]
        public void T12拉平三重循环()
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

        private class TreeNode
        {
            public TreeNode()
            {
                Children = Enumerable.Empty<TreeNode>();
            }

            public int Value { get; set; }
            public IEnumerable<TreeNode> Children { get; set; }
        }

        [Fact]
        public void T13遍历树()
        {
            /**
             * 树结构如下：
             * └─0
             *   ├─1
             *   │ └─3
             *   └─2
             */
            var tree = new TreeNode
            {
                Value = 0,
                Children = new[]
                {
                    new TreeNode
                    {
                        Value = 1,
                        Children = new[]
                        {
                            new TreeNode
                            {
                                Value = 3
                            },
                        }
                    },
                    new TreeNode
                    {
                        Value = 2
                    },
                }
            };

            // 深度优先遍历的结果
            var dftResult = new[] {0, 1, 3, 2};

            // 通过迭代器实现深度优先遍历
            var dft = DFTByEnumerable(tree).ToArray();
            dft.Should().Equal(dftResult);

            // 使用堆栈配合循环算法实现深度优先遍历
            var dftList = DFTByStack(tree).ToArray();
            dftList.Should().Equal(dftResult);

            // 递归算法实现深度优先遍历
            var dftByRecursion = DFTByRecursion(tree).ToArray();
            dftByRecursion.Should().Equal(dftResult);

            // 广度优先遍历的结果
            var bdfResult = new[] {0, 1, 2, 3};

            /**
             * 通过迭代器实现广度优先遍历
             * 此处未提供“通过队列配合循环算法”和“递归算法”实现广度优先遍历的两种算法进行对比。读者可以自行尝试。
             */
            var bft = BFT(tree).ToArray();
            bft.Should().Equal(bdfResult);

            /**
             * 迭代器深度优先遍历
             * depth-first traversal
             */
            IEnumerable<int> DFTByEnumerable(TreeNode root)
            {
                yield return root.Value;
                foreach (var child in root.Children)
                {
                    foreach (var item in DFTByEnumerable(child))
                    {
                        yield return item;
                    }
                }
            }

            // 使用堆栈配合循环算法实现深度优先遍历
            IEnumerable<int> DFTByStack(TreeNode root)
            {
                var result = new List<int>();
                var stack = new Stack<TreeNode>();
                stack.Push(root);
                while (stack.TryPop(out var node))
                {
                    result.Add(node.Value);
                    foreach (var nodeChild in node.Children.Reverse())
                    {
                        stack.Push(nodeChild);
                    }
                }

                return result;
            }

            // 递归算法实现深度优先遍历
            IEnumerable<int> DFTByRecursion(TreeNode root)
            {
                var list = new List<int> {root.Value};
                foreach (var rootChild in root.Children)
                {
                    list.AddRange(DFTByRecursion(rootChild));
                }

                return list;
            }

            // 通过迭代器实现广度优先遍历
            IEnumerable<int> BFT(TreeNode root)
            {
                yield return root.Value;

                foreach (var bftChild in BFTChildren(root.Children))
                {
                    yield return bftChild;
                }

                IEnumerable<int> BFTChildren(IEnumerable<TreeNode> children)
                {
                    var tempList = new List<TreeNode>();
                    foreach (var treeNode in children)
                    {
                        tempList.Add(treeNode);
                        yield return treeNode.Value;
                    }

                    foreach (var bftChild in tempList.SelectMany(treeNode => BFTChildren(treeNode.Children)))
                    {
                        yield return bftChild;
                    }
                }
            }
        }

        [Fact]
        public void T14搜索树()
        {
            /**
             * 此处所指的搜索树是指在遍历树的基础上增加终结遍历的条件。
             * 因为一般构建搜索树是为了找到第一个满足条件的数据，因此与单纯的遍历存在不同。
             * 树结构如下：
             * └─0
             *   ├─1
             *   │ └─3
             *   └─5
             *     └─2
             */

            var tree = new TreeNode
            {
                Value = 0,
                Children = new[]
                {
                    new TreeNode
                    {
                        Value = 1,
                        Children = new[]
                        {
                            new TreeNode
                            {
                                Value = 3
                            },
                        }
                    },
                    new TreeNode
                    {
                        Value = 5,
                        Children = new[]
                        {
                            new TreeNode
                            {
                                Value = 2
                            },
                        }
                    },
                }
            };

            /**
             * 有了深度优先遍历算法的情况下，再增加一个条件判断，便可以实现深度优先的搜索
             * 搜索树中第一个大于等于 3 并且是奇数的数字
             */
            var result = DFS(tree, x => x >= 3 && x % 2 == 1);

            /**
             * 搜索到的结果是3。
             * 特别提出，如果使用广度优先搜索，结果应该是5。
             * 读者可以通过 T13遍历树 中的广度优先遍历算法配合 FirstOrDefault 中相同的条件实现。
             * 建议读者尝试以上代码尝试一下。
             */
            result.Should().Be(3);

            int DFS(TreeNode root, Func<int, bool> predicate)
            {
                var re = DFTByEnumerable(root)
                    .FirstOrDefault(predicate);
                return re;
            }

            // 迭代器深度优先遍历
            IEnumerable<int> DFTByEnumerable(TreeNode root)
            {
                yield return root.Value;
                foreach (var child in root.Children)
                {
                    foreach (var item in DFTByEnumerable(child))
                    {
                        yield return item;
                    }
                }
            }
        }

        [Fact]
        public void T15分页()
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

        [Fact]
        public void T16分页与多级缓存()
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
                    _testOutputHelper.WriteLine("正在从内存中获取数据");
                    return memoryData;
                }

                IEnumerable<int> GetFromRedis()
                {
                    _testOutputHelper.WriteLine("正在从Redis中获取数据");
                    return redisData;
                }

                IEnumerable<int> GetFromElasticSearch()
                {
                    _testOutputHelper.WriteLine("正在从ElasticSearch中获取数据");
                    return elasticSearchData;
                }

                IEnumerable<int> GetFromDatabase()
                {
                    _testOutputHelper.WriteLine("正在从数据库中获取数据");
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
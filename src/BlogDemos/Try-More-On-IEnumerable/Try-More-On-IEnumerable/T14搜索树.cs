namespace Try_More_On_IEnumerable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/2SpQsM
    /// </summary>
    public class T14搜索树
    {
        private class TreeNode
        {
            public TreeNode()
            {
                Children = Enumerable.Empty<TreeNode>();
            }

            /// <summary>
            /// 当前节点的值
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// 当前节点的子节点列表
            /// </summary>
            public IEnumerable<TreeNode> Children { get; set; }
        }

        public static void Run()
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
    }
}
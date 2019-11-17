namespace Try_More_On_IEnumerable
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/bOlMuO
    /// </summary>
    public class T13遍历树
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
    }
}
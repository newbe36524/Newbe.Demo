using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Xunit;
using Xunit.Abstractions;
using System;

namespace Newbe.RxWorld
{
    public class TreeTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TreeTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            var root = CreateTree();
            var nodes = DfsAsEnumerable(root);
            foreach (var treeNode in nodes)
            {
                VisitNode(treeNode);
            }
        }

        [Fact]
        public void ObservableTest()
        {
            var root = CreateTree();
            var observable = DfsAsEnumerable(root).ToObservable();
            observable.Subscribe(VisitNode);
        }

        [Fact]
        public void DfsByRec()
        {
            var root = CreateTree();
            VisitChildren(root);

            void VisitChildren(TreeNode node)
            {
                VisitNode(node);
                foreach (var childNode in node.Children)
                {
                    VisitChildren(childNode);
                }
            }
        }

        [Fact]
        public void DfsByQueue()
        {
            var root = CreateTree();
            var stack = new Stack<TreeNode>();
            stack.Push(root);
            while (stack.Any())
            {
                var node = stack.Pop();
                VisitNode(node);
                foreach (var childNode in node.Children.Reverse())
                {
                    stack.Push(childNode);
                }
            }
        }

        private void VisitNode(TreeNode node)
        {
            _testOutputHelper.WriteLine(node.Name);
        }

        private IEnumerable<TreeNode> DfsAsEnumerable(TreeNode root)
        {
            var stack = new Stack<TreeNode>();
            stack.Push(root);
            while (stack.Any())
            {
                var node = stack.Pop();
                yield return node;
                foreach (var childNode in node.Children.Reverse())
                {
                    stack.Push(childNode);
                }
            }
        }

        private TreeNode CreateTree()
        {
            return new TreeNode
            {
                Name = "root",
                Children = new[]
                {
                    new TreeNode
                    {
                        Name = "node1",
                        Children = new[]
                        {
                            new TreeNode
                            {
                                Name = "node2",
                                Children = Enumerable.Empty<TreeNode>()
                            },
                        }
                    },
                    new TreeNode
                    {
                        Name = "node3",
                        Children = Enumerable.Empty<TreeNode>()
                    },
                }
            };
        }
    }

    public class TreeNode
    {
        public string Name { get; set; }
        public IEnumerable<TreeNode> Children { get; set; }
    }
}
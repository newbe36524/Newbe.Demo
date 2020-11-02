using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Newbe.ExpressionsTests
{
    public class FactoryTest
    {
        [Test]
        public void CreateTreeNodeTest()
        {
            var dic = new Dictionary<Type, HashSet<Type>>
            {
                {
                    typeof(IConfigProvider), new HashSet<Type>
                    {
                        typeof(ConfigProvider)
                    }
                },
                {
                    typeof(ISmsSenderFactory), new HashSet<Type>
                    {
                        typeof(SmsSenderFactory)
                    }
                },
                {
                    typeof(ISmsSenderFactoryHandler), new HashSet<Type>
                    {
                        typeof(SmsSenderFactoryHandler), typeof(ConsoleSmsSenderFactoryHandler)
                    }
                },
            };
            var nodeDic = TreeHelper.CreateTree(dic);

            Console.WriteLine(nodeDic);
        }


        [Test]
        public void DpTest()
        {
            var root = new TreeNode
            {
                InterfaceType = typeof(string),
                Children = new List<TreeNode>
                {
                    new TreeNode
                    {
                        InterfaceType = typeof(int),
                        Children = new List<TreeNode>
                        {
                            new TreeNode
                            {
                                InterfaceType = typeof(short)
                            }
                        }
                    },
                    new TreeNode
                    {
                        InterfaceType = typeof(long),
                        Children = new List<TreeNode>
                        {
                            new TreeNode
                            {
                                InterfaceType = typeof(double)
                            }
                        }
                    }
                }
            };

            var stack = new Stack<TreeNode>();
            var set = new HashSet<TreeNode>();
            stack.Push(root);
            while (stack.TryPeek(out var node))
            {
                if (!set.Contains(node))
                {
                    set.Add(node);
                    if (node.Children != null)
                    {
                        foreach (var child in node.Children)
                        {
                            stack.Push(child);
                            set.Add(root);
                        }
                    }
                    else
                    {
                        Console.WriteLine(node.InterfaceType.Name);
                        stack.Pop();
                    }
                }
                else
                {
                    Console.WriteLine(node.InterfaceType.Name);
                    stack.Pop();
                }
            }
        }

        [Test]
        public void Run()
        {
            var builder = new MyContainerBuilder();
            builder.Register<UserDal, IUserDal>();
            builder.Register<UserBll, IUserBll>();
            builder.Register<SmsSenderFactory, ISmsSenderFactory>();
            builder.Register<ConsoleSmsSenderFactoryHandler, ISmsSenderFactoryHandler>();
            builder.Register<SmsSenderFactoryHandler, ISmsSenderFactoryHandler>();
            builder.Register<ConfigProvider, IConfigProvider>();

            var objectFactory = builder.Build();
            var userBll = objectFactory.Resolve<IUserBll>();
            var login = userBll.Login("yueluo", "newbe");
            Console.WriteLine(login);

            login = userBll.Login("newbe", "yueluo");
            Console.WriteLine(login);
        }
    }
}
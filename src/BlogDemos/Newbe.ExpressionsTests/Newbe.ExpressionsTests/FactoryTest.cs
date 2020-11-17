using System;
using System.Collections.Generic;
using FluentAssertions;
using Newbe.ExpressionsTests.Impl;
using Newbe.ExpressionsTests.Interfaces;
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
                TargetType = typeof(string),
                Children = new List<TreeNode>
                {
                    new TreeNode
                    {
                        TargetType = typeof(int),
                        Children = new List<TreeNode>
                        {
                            new TreeNode
                            {
                                TargetType = typeof(short)
                            }
                        }
                    },
                    new TreeNode
                    {
                        TargetType = typeof(long),
                        Children = new List<TreeNode>
                        {
                            new TreeNode
                            {
                                TargetType = typeof(double)
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
                        Console.WriteLine(node.TargetType.Name);
                        stack.Pop();
                    }
                }
                else
                {
                    Console.WriteLine(node.TargetType.Name);
                    stack.Pop();
                }
            }
        }

        [Test]
        public void ResolveSelf()
        {
            var builder = new MyContainerBuilder();
            builder.Register<MyLogger, MyLogger>();
            builder.Register<MyLogger, IMyLogger>();
            builder.Register<MyConsoleLogger, IMyLogger>();

            var objectFactory = builder.Build();
            {
                var logger = objectFactory.Resolve<MyLogger>();
                logger.Should().NotBeNull();
            }
            {
                var logger = objectFactory.Resolve<IMyLogger>();
                logger.Should().BeOfType<MyConsoleLogger>();
            }
            {
                var loggers = objectFactory.Resolve<IMyLogger[]>();
                loggers.Length.Should().Be(2);
            }
            {
                var loggerFactory = objectFactory.Resolve<Func<IMyLogger>>();
                var logger = loggerFactory.Invoke();
                logger.Should().BeOfType<MyConsoleLogger>();
            }
            {
                var loggerFactory = objectFactory.Resolve<Func<IMyLogger[]>>();
                var loggers = loggerFactory.Invoke();
                loggers.Length.Should().Be(2);
            }
            {
                var loggerFactory = objectFactory.Resolve<Func<Func<IMyLogger>>>();
                var logger = loggerFactory()();
                logger.Should().BeOfType<MyConsoleLogger>();
            }
        }

        [Test]
        public void Run()
        {
            var builder = new MyContainerBuilder();
            builder.Register<MyLogger, IMyLogger>();
            builder.Register<ConsoleSmsSender, ConsoleSmsSender>();
            builder.Register<UserDal, IUserDal>();
            builder.Register<UserBll, IUserBll>();
            builder.Register<SmsSenderFactory, ISmsSenderFactory>();
            builder.Register<ConsoleSmsSenderFactoryHandler, ISmsSenderFactoryHandler>(SmsSenderType.Console);
            builder.Register<SmsSenderFactoryHandler, ISmsSenderFactoryHandler>(SmsSenderType.HttpAPi);
            builder.Register<ConfigProvider, IConfigProvider>();

            var objectFactory = builder.Build();
            var userBll = objectFactory.Resolve<IUserBll>();
            var login = userBll.Login("yueluo", "newbe");
            Console.WriteLine(login);

            login = userBll.Login("newbe", "yueluo");
            Console.WriteLine(login);
        }
    }

    public interface IMyIndex<in TKey, out TInterface>
    {
        TInterface this[TKey key] { get; }
    }

    public class MyIndex<TKey, TInterface> : IMyIndex<TKey, TInterface>
    {
        private readonly Dictionary<TKey, Func<TInterface>> _handlers;

        public MyIndex(
            Dictionary<TKey, Func<TInterface>> handlers)
        {
            _handlers = handlers;
        }

        public TInterface this[TKey key] => _handlers[key].Invoke();
    }
}
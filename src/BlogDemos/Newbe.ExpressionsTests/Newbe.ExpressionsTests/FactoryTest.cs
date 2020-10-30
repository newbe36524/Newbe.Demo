using System;
using System.Collections.Generic;
using System.Linq;
using Newbe.ExpressionsTests.Impl;
using Newbe.ExpressionsTests.Interfaces;
using NUnit.Framework;

namespace Newbe.ExpressionsTests
{
    public class FactoryTest
    {
        public class TreeNode
        {
            public Type InterfaceType { get; set; }
            public Type ImplType { get; set; }
            public TreeNode Parent { get; set; }
            public List<TreeNode> Children { get; set; }
        }

        [Test]
        public void CreateTreeNodeTest()
        {
            var dic = new Dictionary<Type, Type>
            {
                {typeof(IStudentBll), typeof(StudentBll)},
                {typeof(IStudentDal), typeof(StudentDal1)},
            };

            // a -> b,c
            // b ->  
            // c -> d
            // d ->

            // b <- a
            // c <- a
            // d <- c

            var dependDic = dic.ToDictionary(x => x.Value, x =>
            {
                var constructorInfo = x.Value.GetConstructors().FirstOrDefault();
                if (constructorInfo == null)
                {
                    return Array.Empty<Type>();
                }

                return constructorInfo.GetParameters()
                    .Select(a => a.ParameterType)
                    .ToArray();
            });

            var newDic = new Dictionary<Type, List<Type>>();

            foreach (var (implType, dependItems) in dependDic)
            {
                foreach (var dependItem in dependItems)
                {
                    if (!newDic.TryGetValue(dependItem, out var list))
                    {
                        list = new List<Type>();
                    }

                    list.Add(implType);
                    newDic[dependItem] = list;
                }
            }

            var nodeDic = dic
                .Select(x => new TreeNode
                {
                    InterfaceType = x.Key,
                    ImplType = x.Value,
                    Children = new List<TreeNode>()
                })
                .ToDictionary(x => x.ImplType);

            foreach (var node in nodeDic.Values)
            {
                if (newDic.TryGetValue(node.InterfaceType, out var list))
                {
                    foreach (var implType in list)
                    {
                        var parent = nodeDic[implType];
                        node.Parent = parent;
                        parent.Children.Add(node);
                    }
                }
            }

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
            Console.WriteLine($"开始运行{nameof(FactoryTest)}");
            // 使用 StudentDal1
            var builder1 = new MyContainerBuilder();
            builder1.Register<StudentDal1, IStudentDal>();
            builder1.Register<StudentBll, IStudentBll>();
            builder1.Register<MyLogger, IMyLogger>();

            var factory1 = builder1.Build();
            var studentBll = factory1.Resolve<IStudentBll>();
            var students = studentBll.GetStudents();
            foreach (var student in students)
            {
                Console.WriteLine(student);
            }

            // 使用 StudentDal2
            var builder2 = new MyContainerBuilder();
            builder2.Register<MyLogger, IMyLogger>();
            builder2.Register<StudentBll, IStudentBll>();
            builder2.Register<StudentDal2, IStudentDal>();
            var factory = builder2.Build();
            studentBll = factory.Resolve<IStudentBll>();

            students = studentBll.GetStudents();
            foreach (var student in students)
            {
                Console.WriteLine(student);
            }

            Console.WriteLine($"结束运行{nameof(FactoryTest)}");
        }
    }
}
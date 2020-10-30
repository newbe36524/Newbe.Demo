using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AgileObjects.ReadableExpressions;

namespace Newbe.ExpressionsTests
{
    public class SimpleObjectFactoryHandler : IObjectFactoryHandler
    {
        /// <summary>
        ///  interfaceType: implType
        /// </summary>
        public Dictionary<Type, Type> TypeMapping { get; set; } = new Dictionary<Type, Type>();

        /// <summary>
        ///  interfaceType: Func
        /// </summary>
        private readonly Dictionary<Type, Func<object>> _factories = new Dictionary<Type, Func<object>>();

        /// <summary>
        ///  interfaceType: newExp
        /// </summary>
        private readonly Dictionary<Type, Expression> _finalExp = new Dictionary<Type, Expression>();

        public IObjectFactory ObjectFactory { get; set; }

        public class TreeNode
        {
            public Type InterfaceType { get; set; }
            public Type ImplType { get; set; }
            public TreeNode Parent { get; set; }
            public List<TreeNode> Children { get; set; }
        }

        private TreeNode[] CreateTree()
        {
            var dic = TypeMapping;
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

            var roots = nodeDic.Values.Where(x => x.Parent == null);
            return roots.ToArray();
        }

        public void InitFunc()
        {
            var treeNodes = CreateTree();
            foreach (var root in treeNodes)
            {
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
                            CreateData(node);
                            stack.Pop();
                        }
                    }
                    else
                    {
                        CreateData(node);
                        stack.Pop();
                    }
                }
            }

            void CreateData(TreeNode node)
            {
                if (!_factories.ContainsKey(node.InterfaceType))
                {
                    var newExp = CreateNewExp(node.InterfaceType, node.ImplType);
                    _factories[node.InterfaceType] = CreateFactory(newExp);
                    _finalExp[node.InterfaceType] = newExp;
                    Console.WriteLine(newExp.ToReadableString());
                }
            }
        }

        public bool CanHandle(Type waitingType)
        {
            return TypeMapping.ContainsKey(waitingType);
        }

        public object Resolve(Type waitingType)
        {
            var factory = _factories[waitingType];
            var re = factory.Invoke();
            return re;
        }

        private Func<object> CreateFactory(Expression newExp)
        {
            var finalExp = Expression.Lambda<Func<object>>(newExp);
            var func = finalExp.Compile();
            return func;
        }

        private Expression CreateNewExp(Type interfaceType, Type implType)
        {
            var constructorInfo = implType.GetConstructors().FirstOrDefault();
            Expression newExp;
            if (constructorInfo == null)
            {
                newExp = Expression.New(implType);
                _finalExp[interfaceType] = newExp;
            }
            else
            {
                var parameterInfos = constructorInfo.GetParameters();
                List<Expression> list = new List<Expression>();
                var waitingTypes = new Stack<Type>();
                foreach (var parameterInfo in parameterInfos)
                {
                    waitingTypes.Push(parameterInfo.ParameterType);
                }

                while (waitingTypes.TryPop(out var waitingOne))
                {
                    var pNewExp = _finalExp[waitingOne];
                    list.Insert(0, pNewExp);
                }

                newExp = Expression.New(constructorInfo, list);
            }

            return newExp;
        }
    }
}
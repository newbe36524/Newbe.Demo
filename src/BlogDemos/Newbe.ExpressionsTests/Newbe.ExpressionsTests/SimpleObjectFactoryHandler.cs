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
        public Dictionary<Type, HashSet<Type>> TypeMapping { get; set; } = new Dictionary<Type, HashSet<Type>>();

        /// <summary>
        ///  interfaceType: Func
        /// </summary>
        private readonly Dictionary<Type, List<Func<object>>> _factories
            = new Dictionary<Type, List<Func<object>>>();

        /// <summary>
        ///  interfaceType: newExp
        /// </summary>
        private readonly Dictionary<Type, List<Expression>> _instanceExpressions
            = new Dictionary<Type, List<Expression>>();

        private TreeNode[] CreateTree()
        {
            var treeNodes = TreeHelper.CreateTree(TypeMapping);
            var roots = treeNodes.Where(x => x.Parent == null);
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
                var nodeInterfaceType = node.InterfaceType;
                var newExp = CreateNewExp(nodeInterfaceType, node.ImplType);
                CreateOrAddItem(_factories, nodeInterfaceType, CreateFactory(newExp));
                CreateOrAddItem(_instanceExpressions, nodeInterfaceType, newExp);

                Console.WriteLine(newExp.ToReadableString());
            }
        }

        public static void CreateOrAddItem<T>(Dictionary<Type, List<T>> dic, Type key, T item)
        {
            if (!dic.TryGetValue(key, out var list))
            {
                list = new List<T>();
            }

            list.Add(item);
            dic[key] = list;
        }

        public bool CanHandle(Type waitingType)
        {
            return TypeMapping.ContainsKey(waitingType);
        }

        public object Resolve(Type waitingType)
        {
            var factory = CreateDefaultInstanceSelector(_factories[waitingType]);
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
                CreateOrAddItem(_instanceExpressions, interfaceType, newExp);
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
                    Expression pExp;
                    if (waitingOne.IsArray)
                    {
                        var elementType = TreeHelper.GetArrayElementType(waitingOne);
                        var pNewExp = _instanceExpressions[elementType];
                        pExp = Expression.NewArrayInit(elementType, pNewExp);
                    }
                    else
                    {
                        var pNewExp = _instanceExpressions[waitingOne];
                        pExp = CreateDefaultInstanceSelector(pNewExp);
                    }

                    list.Insert(0, pExp);
                }

                newExp = Expression.New(constructorInfo, list);
            }

            return newExp;
        }

        public static T CreateDefaultInstanceSelector<T>(IEnumerable<T> source)
        {
            return source.First();
        }
    }
}
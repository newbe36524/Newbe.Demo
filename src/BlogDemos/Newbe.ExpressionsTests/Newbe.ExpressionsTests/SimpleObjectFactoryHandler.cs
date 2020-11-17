using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using AgileObjects.ReadableExpressions;

namespace Newbe.ExpressionsTests
{
    public class SimpleObjectFactoryHandler : IObjectFactoryHandler
    {
        private readonly IResolveHandler[] _handlers;

        public SimpleObjectFactoryHandler()
        {
            _handlers = new IResolveHandler[]
            {
                new EnumerableResolveHandler(),
                new MyIndexResolveHandler(),
                new DelegateFactoryResolveHandler(ResolveNewExpression),
                new DefaultResolveHandler(),
            };
        }

        /// <summary>
        ///  interfaceType: implType
        /// </summary>
        public Dictionary<Type, HashSet<TypeRegistrationItem>> TypeMapping { get; set; } =
            new Dictionary<Type, HashSet<TypeRegistrationItem>>();

        /// <summary>
        ///  interfaceType: Func
        /// </summary>
        private readonly Dictionary<Type, List<Func<object>>> _factories
            = new Dictionary<Type, List<Func<object>>>();

        /// <summary>
        ///  interfaceType: newExp
        /// </summary>
        private readonly Dictionary<Type, Dictionary<Expression, MyMedata>> _instanceExpressions
            = new Dictionary<Type, Dictionary<Expression, MyMedata>>();

        private TreeNode[] CreateTree()
        {
            var typeDic = new Dictionary<Type, HashSet<Type>>();
            foreach (var (k, v) in TypeMapping)
            {
                typeDic[k] = v.Select(x => x.ImplType).ToHashSet();
            }

            var treeNodes = TreeHelper.CreateTree(typeDic);
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
                var nodeInterfaceType = node.TargetType;
                var newExp = CreateNewExp(nodeInterfaceType, node.ImplType);
                CreateOrAddItem(_factories, nodeInterfaceType, CreateFactory(newExp));
                // CreateOrAddItem(_instanceExpressions, nodeInterfaceType, newExp);

                if (!_instanceExpressions.TryGetValue(nodeInterfaceType, out var d))
                {
                    d = new Dictionary<Expression, MyMedata>();
                }

                d[newExp] = GetMetaData(nodeInterfaceType, node.ImplType)!;
                _instanceExpressions[nodeInterfaceType] = d;
                Console.WriteLine(newExp.ToReadableString());
            }
        }

        private MyMedata GetMetaData(Type interfaceType, Type implType)
        {
            return TypeMapping[interfaceType].Single(x => x.ImplType == implType).Medata;
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
            foreach (var handler in _handlers)
            {
                var canHandle = handler.CanHandle(waitingType);
                if (canHandle)
                {
                    return true;
                }
            }

            return false;
        }

        private static readonly ConcurrentDictionary<Type, Func<object>> StaticFactoires =
            new ConcurrentDictionary<Type, Func<object>>();

        public object Resolve(Type waitingType)
        {
            foreach (var handler in _handlers)
            {
                var canHandle = handler.CanHandle(waitingType);
                if (canHandle)
                {
                    var func = StaticFactoires.GetOrAdd(waitingType, ValueFactory);
                    var re = func.Invoke();
                    return re;

                    Func<object> ValueFactory(Type arg)
                    {
                        var exp = handler.Resolve(waitingType, _instanceExpressions);
                        var finalExp = Expression.Lambda<Func<object>>(exp);
                        var factory = finalExp.Compile();
                        return factory;
                    }
                }
            }


            throw new NotSupportedException($"Can not resolve type : {waitingType}");
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
                if (!_instanceExpressions.TryGetValue(interfaceType, out var d))
                {
                    d = new Dictionary<Expression, MyMedata>();
                }

                d[newExp] = GetMetaData(interfaceType, implType)!;
                _instanceExpressions[interfaceType] = d;
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
                    var pExp = ResolveNewExpression(waitingOne);
                    Debug.Assert(pExp != null, nameof(pExp) + " != null");
                    list.Insert(0, pExp);
                }

                newExp = Expression.New(constructorInfo, list);
            }

            return newExp;
        }

        private Expression ResolveNewExpression(Type waitingOne)
        {
            Expression? pExp = null;
            foreach (var handler in _handlers)
            {
                var canHandle = handler.CanHandle(waitingOne);
                if (canHandle)
                {
                    var exp = handler.Resolve(waitingOne, _instanceExpressions);
                    pExp = exp;
                    break;
                }
            }

            return pExp;
        }

        public interface IResolveHandler
        {
            bool CanHandle(Type type);
            Expression Resolve(Type type, Dictionary<Type, Dictionary<Expression, MyMedata>> instanceExps);
        }


        private class DelegateFactoryResolveHandler : IResolveHandler
        {
            private readonly Func<Type, Expression> _newExpressionFactory;

            public DelegateFactoryResolveHandler(
                Func<Type, Expression> newExpressionFactory)
            {
                _newExpressionFactory = newExpressionFactory;
            }

            public bool CanHandle(Type type)
            {
                if (type.BaseType == null)
                {
                    return false;
                }

                return type.BaseType.BaseType == typeof(Delegate);
            }

            public Expression Resolve(Type type, Dictionary<Type, Dictionary<Expression, MyMedata>> instanceExps)
            {
                var returnType = type.GetMethod("Invoke").ReturnType;
                var expression = _newExpressionFactory.Invoke(returnType);
                return Expression.Lambda(type, expression);
            }
        }

        private class DefaultResolveHandler : IResolveHandler
        {
            public bool CanHandle(Type type)
            {
                return true;
            }

            public Expression Resolve(Type type, Dictionary<Type, Dictionary<Expression, MyMedata>> instanceExps)
            {
                var pNewExp = instanceExps[type];
                var pExp = CreateDefaultInstanceSelector(pNewExp);
                return pExp.Key;
            }
        }


        private class MyIndexResolveHandler : IResolveHandler
        {
            public bool CanHandle(Type type)
            {
                return type.Name == typeof(IMyIndex<,>).Name;
            }

            public Expression Resolve(Type type, Dictionary<Type, Dictionary<Expression, MyMedata>> instanceExps)
            {
                var interfaceType = type.GenericTypeArguments[1];
                var keyType = type.GenericTypeArguments[0];
                var instanceExp = instanceExps[interfaceType];
                var keyValuePairs = instanceExp
                    .Where(x => x.Value != null)
                    .Where(x => x.Value.Key.GetType() == keyType);
                var funcType = Expression.GetFuncType(interfaceType);
                var lambdaExpressions =
                    keyValuePairs.ToDictionary(x => x.Value, x => (Expression) Expression.Lambda(funcType, x.Key));
                var methodCallExpression = Expression.Call(typeof(MyIndexResolveHandler),
                    nameof(Create),
                    new[] {keyType, interfaceType},
                    Expression.Constant(lambdaExpressions));
                return methodCallExpression;
            }

            public static IMyIndex<TKey, TService> Create<TKey, TService>(
                Dictionary<MyMedata, Expression> items)
            {
                var dictionary = items.ToDictionary(x => (TKey) x.Key.Key,
                    x => ((Expression<Func<TService>>) x.Value).Compile());
                return new MyIndex<TKey, TService>(dictionary);
            }
        }


        private class EnumerableResolveHandler : IResolveHandler
        {
            public bool CanHandle(Type type)
            {
                var isEnumerable = type.Name == typeof(IEnumerable<>).Name;
                if (isEnumerable)
                {
                    return true;
                }

                return type.GetInterfaces()
                    .Any(x => x.Name == typeof(IEnumerable<>).Name);
            }

            public Expression Resolve(Type type, Dictionary<Type, Dictionary<Expression, MyMedata>> instanceExps)
            {
                Type elementType;
                if (type.Name == typeof(IEnumerable<>).Name)
                {
                    elementType = type.GenericTypeArguments[0];
                }
                else
                {
                    elementType = type.GetInterfaces()
                        .First(x => x.Name == typeof(IEnumerable<>).Name)
                        .GenericTypeArguments[0];
                }

                var pNewExp = instanceExps[elementType];
                Expression? re;
                if (type.BaseType == typeof(Array))
                {
                    re = Expression.NewArrayInit(elementType, pNewExp.Keys);
                }
                else if (type.Name == typeof(List<>).Name)
                {
                    re = Expression.Call(typeof(EnumerableResolveHandler),
                        nameof(NewList),
                        new[] {elementType},
                        Expression.NewArrayInit(elementType, pNewExp.Keys));
                }
                else if (type.Name == typeof(HashSet<>).Name)
                {
                    re = Expression.Call(typeof(EnumerableResolveHandler),
                        nameof(NewHashSet),
                        new[] {elementType},
                        Expression.NewArrayInit(elementType, pNewExp.Keys));
                }
                else
                {
                    re = Expression.NewArrayInit(elementType, pNewExp.Keys);
                }

                return re;
            }

            public static List<T> NewList<T>(T[] source)
            {
                var list = new List<T>(source.Length);
                return list;
            }

            public static HashSet<T> NewHashSet<T>(T[] source)
            {
                var list = new HashSet<T>(source.Length);
                return list;
            }
        }


        public static T CreateDefaultInstanceSelector<T>(IEnumerable<T> source)
        {
            return source.Last();
        }
    }
}
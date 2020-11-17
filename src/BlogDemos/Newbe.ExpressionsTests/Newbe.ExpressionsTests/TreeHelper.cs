using System;
using System.Collections.Generic;
using System.Linq;

namespace Newbe.ExpressionsTests
{
    public static class TreeHelper
    {
        public static TreeNode[] CreateTree(Dictionary<Type, HashSet<Type>> dic)
        {
            var dependDic = dic.Values
                .SelectMany(x => x)
                .Distinct()
                .ToDictionary(x => x,
                    x =>
                    {
                        var constructorInfo = x.GetConstructors().FirstOrDefault();
                        if (constructorInfo == null)
                        {
                            return Array.Empty<Type>();
                        }

                        return constructorInfo.GetParameters()
                            .Select(a => a.ParameterType)
                            .ToArray();
                    });

            // ISmsSenderFactoryHandler[] :SmsSenderFactory
            var newDic = dependDic
                .SelectMany(x => x.Value.Select(a => new
                {
                    ImplType = x.Key,
                    DependencyItemType = a
                }))
                .ToLookup(x => x.DependencyItemType)
                .ToDictionary(x => x.Key, x => x.ToList());


            // IConfigProvider:ConfigProvider
            // ISmsSenderFactoryHandler:SmsSenderFactoryHandler
            // ISmsSenderFactoryHandler:ConsoleSmsSenderFactoryHandler
            // ISmsSenderFactory:SmsSenderFactory 
            var nodeDic = dic
                .SelectMany(x => x.Value.Select(implType => new TreeNode
                {
                    TargetType = x.Key,
                    ImplType = implType,
                    Children = new List<TreeNode>()
                }))
                .ToArray();

            foreach (var (beiyilaixiao, yilaixiang) in newDic)
            {
                if (IsArrayType(beiyilaixiao))
                {
                    var elementInterfaceType = GetArrayElementType(beiyilaixiao);
                    ConcatNode(elementInterfaceType);
                }
                else
                {
                    ConcatNode(beiyilaixiao);
                }

                void ConcatNode(Type interfaceType)
                {
                    var childrenNode = nodeDic
                        .Where(x => x.TargetType == interfaceType)
                        .ToArray();
                    foreach (var item in yilaixiang)
                    {
                        foreach (var treeNode in nodeDic.Where(x => x.ImplType == item.ImplType))
                        {
                            treeNode.Children.AddRange(childrenNode);
                            foreach (var child in childrenNode)
                            {
                                child.Parent = treeNode;
                            }
                        }
                    }
                }
            }

            bool IsArrayType(Type type)
            {
                return type.IsArray;
            }

            return nodeDic;
        }

        public static Type GetArrayElementType(Type type)
        {
            var elType = type.GetInterfaces()
                .First(x => x.Name == typeof(IEnumerable<>).Name)
                .GenericTypeArguments[0];
            return elType;
        }
    }
}
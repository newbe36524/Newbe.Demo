using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Newbe.ExpressionsTests.Impl;
using Newbe.ExpressionsTests.Interfaces;
using NUnit.Framework;

namespace Newbe.ExpressionsTests
{
    public interface IObjectFactory
    {
        object Resolve(Type resolvingType);
    }

    public static class ObjectFactoryExtensions
    {
        public static T Resolve<T>(this IObjectFactory factory)
        {
            var obj = factory.Resolve(typeof(T));
            return (T) obj;
        }
    }

    public interface IObjectFactoryHandler
    {
        bool CanHandle(Type waitingType);
        object Resolve(Type waitingType);
    }

    public interface IObjectFactoryRegister
    {
        void Register(Type implType, Type interfaceType);
    }

    public class SimpleObjectFactoryHandler : IObjectFactoryRegister, IObjectFactoryHandler
    {
        public Dictionary<Type, Type> TypeMapping { get; set; } = new Dictionary<Type, Type>();
        public IObjectFactory ObjectFactory { get; set; }

        public bool CanHandle(Type waitingType)
        {
            return TypeMapping.ContainsKey(waitingType);
        }

        public object Resolve(Type waitingType)
        {
            var implType = TypeMapping[waitingType];
            var constructorInfo = implType.GetConstructors().FirstOrDefault();
            Expression newExp;
            if (constructorInfo == null)
            {
                newExp = Expression.New(implType);
            }
            else
            {
                var parameterInfos = constructorInfo.GetParameters();
                var factoryExp = Expression.Constant(ObjectFactory);

                List<Expression> list = new List<Expression>();
                foreach (var parameterInfo in parameterInfos)
                {
                    var pExp = Expression.Call(typeof(ObjectFactoryExtensions),
                        nameof(ObjectFactoryExtensions.Resolve),
                        new[] {parameterInfo.ParameterType},
                        factoryExp);
                    list.Add(pExp);
                }

                newExp = Expression.New(constructorInfo, list);
            }

            var finalExp = Expression.Lambda<Func<object>>(newExp);
            var func = finalExp.Compile();
            var re = func.Invoke();
            return re;
        }

        public void Register(Type implType, Type interfaceType)
        {
            TypeMapping[interfaceType] = implType;
        }
    }


    public interface IMyContainerBuilder
    {
        IObjectFactory Build();
    }

    public class MyContainerBuilder : IMyContainerBuilder
    {
        private readonly Dictionary<Type, Type> _dictionary = new Dictionary<Type, Type>();

        public void Register<TImpl, TInterface>()
        {
            _dictionary[typeof(TInterface)] = typeof(TImpl);
        }

        public IObjectFactory Build()
        {
            var simpleObjectFactoryHandler = new SimpleObjectFactoryHandler {TypeMapping = _dictionary};
            var objectFactory = new ObjectFactory(new[]
            {
                simpleObjectFactoryHandler
            });
            simpleObjectFactoryHandler.ObjectFactory = objectFactory;
            return objectFactory;
        }
    }

    public class ObjectFactory : IObjectFactory
    {
        private readonly IEnumerable<IObjectFactoryHandler> _handlers;

        public ObjectFactory(IEnumerable<IObjectFactoryHandler> handlers)
        {
            _handlers = handlers;
        }

        public object Resolve(Type resolvingType)
        {
            foreach (var handler in _handlers)
            {
                var canHandle = handler.CanHandle(resolvingType);
                if (canHandle)
                {
                    var obj = handler.Resolve(resolvingType);
                    return obj;
                }
            }

            throw new MissingObjectException(resolvingType);
        }
    }

    public class FactoryTest
    {
        [Test]
        public void Run()
        {
            Console.WriteLine($"开始运行{nameof(FactoryTest)}");
            // 使用 StudentDal1
            var builder = new MyContainerBuilder();
            builder.Register<StudentBll, IStudentBll>();
            builder.Register<StudentDal1, IStudentDal>();

            var factory = builder.Build();
            var studentBll = factory.Resolve<IStudentBll>();
            var students = studentBll.GetStudents();
            foreach (var student in students)
            {
                Console.WriteLine(student);
            }

            // 使用 StudentDal2
            studentBll = new StudentBll(new StudentDal2());
            students = studentBll.GetStudents();
            foreach (var student in students)
            {
                Console.WriteLine(student);
            }

            Console.WriteLine($"结束运行{nameof(FactoryTest)}");
        }
    }
}
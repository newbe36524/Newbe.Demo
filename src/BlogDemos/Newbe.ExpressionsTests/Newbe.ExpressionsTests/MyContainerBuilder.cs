using System;
using System.Collections.Generic;

namespace Newbe.ExpressionsTests
{
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
            simpleObjectFactoryHandler.InitFunc();
            return objectFactory;
        }
    }
}
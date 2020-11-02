using System;
using System.Collections.Generic;

namespace Newbe.ExpressionsTests
{
    public class MyContainerBuilder : IMyContainerBuilder
    {
        private readonly Dictionary<Type, HashSet<Type>> _dictionary = new Dictionary<Type, HashSet<Type>>();

        public void Register<TImpl, TInterface>()
        {
            var interfaceType = typeof(TInterface);
            if (!_dictionary.TryGetValue(interfaceType, out var set))
            {
                set = new HashSet<Type>();
            }

            set.Add(typeof(TImpl));
            _dictionary[interfaceType] = set;
        }

        public IObjectFactory Build()
        {
            var simpleObjectFactoryHandler = new SimpleObjectFactoryHandler {TypeMapping = _dictionary};
            var objectFactory = new ObjectFactory(new[]
            {
                simpleObjectFactoryHandler
            });
            simpleObjectFactoryHandler.InitFunc();
            return objectFactory;
        }
    }
}
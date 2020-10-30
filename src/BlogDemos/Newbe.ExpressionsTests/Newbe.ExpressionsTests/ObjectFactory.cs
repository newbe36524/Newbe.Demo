using System;
using System.Collections.Generic;

namespace Newbe.ExpressionsTests
{
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
}
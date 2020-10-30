using System;

namespace Newbe.ExpressionsTests
{
    public interface IObjectFactoryHandler
    {
        bool CanHandle(Type waitingType);
        object Resolve(Type waitingType);
    }
}
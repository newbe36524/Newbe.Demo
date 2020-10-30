using System;

namespace Newbe.ExpressionsTests
{
    public interface IObjectFactory
    {
        object Resolve(Type resolvingType);
    }
}
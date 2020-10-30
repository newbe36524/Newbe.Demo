using System;

namespace Newbe.ExpressionsTests
{
    public interface IObjectFactoryRegister
    {
        void Register(Type implType, Type interfaceType);
    }
}
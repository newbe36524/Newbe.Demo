using System;
using Newbe.ExpressionsTests.Old.X10.Model;

namespace Newbe.ExpressionsTests.Old.X10.Interfaces
{
    public interface IValidatorFactory
    {
        Func<object, ValidateResult> GetValidator(Type type);
    }
}
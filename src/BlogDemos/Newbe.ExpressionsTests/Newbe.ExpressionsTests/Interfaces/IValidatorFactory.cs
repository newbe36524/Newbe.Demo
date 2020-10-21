using System;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Interfaces
{
    public interface IValidatorFactory
    {
        Func<object, ValidateResult> GetValidator(Type type);
    }
}
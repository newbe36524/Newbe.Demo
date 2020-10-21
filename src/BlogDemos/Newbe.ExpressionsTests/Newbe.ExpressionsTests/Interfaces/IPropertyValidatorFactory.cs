using System.Collections.Generic;
using System.Linq.Expressions;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Interfaces
{
    public interface IPropertyValidatorFactory
    {
        IEnumerable<Expression> CreateExpression(CreatePropertyValidatorInput input);
    }
}
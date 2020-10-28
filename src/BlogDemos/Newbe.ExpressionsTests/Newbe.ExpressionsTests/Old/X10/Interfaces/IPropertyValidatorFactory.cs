using System.Collections.Generic;
using System.Linq.Expressions;
using Newbe.ExpressionsTests.Old.X10.Model;

namespace Newbe.ExpressionsTests.Old.X10.Interfaces
{
    public interface IPropertyValidatorFactory
    {
        IEnumerable<Expression> CreateExpression(CreatePropertyValidatorInput input);
    }
}
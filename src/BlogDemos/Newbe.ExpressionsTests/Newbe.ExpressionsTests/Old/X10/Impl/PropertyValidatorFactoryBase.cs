using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newbe.ExpressionsTests.Old.X10.Interfaces;
using Newbe.ExpressionsTests.Old.X10.Model;

namespace Newbe.ExpressionsTests.Old.X10.Impl
{
    public abstract class PropertyValidatorFactoryBase<TValue> : IPropertyValidatorFactory
    {
        public virtual IEnumerable<Expression> CreateExpression(CreatePropertyValidatorInput input)
        {
            if (input.PropertyInfo.PropertyType != typeof(TValue))
            {
                return Enumerable.Empty<Expression>();
            }

            var expressionCore = CreateExpressionCore(input);
            return expressionCore;
        }

        protected abstract IEnumerable<Expression> CreateExpressionCore(CreatePropertyValidatorInput input);
    }
}
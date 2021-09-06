using System;
using System.Linq.Expressions;

namespace Newbe.ExpressionsTests.FilterFactory
{
    public interface IFilterExpressionFactory<T>
    {
        Expression<Func<T, bool>> CreateExpression(IFilterNodeRelation<T> root);
    }
}
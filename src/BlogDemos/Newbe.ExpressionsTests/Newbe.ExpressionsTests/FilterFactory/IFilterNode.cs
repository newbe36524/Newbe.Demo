using System;
using System.Linq.Expressions;

namespace Newbe.ExpressionsTests.FilterFactory
{
    public interface IFilterNode<T>
    {
        Expression<Func<T, bool>> CreateExpression();
    }
}
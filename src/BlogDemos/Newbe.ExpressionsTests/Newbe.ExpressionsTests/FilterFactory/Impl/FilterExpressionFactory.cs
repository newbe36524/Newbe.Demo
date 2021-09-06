using System;
using System.Linq.Expressions;

namespace Newbe.ExpressionsTests.FilterFactory.Impl
{
    public class FilterExpressionFactory<T> : IFilterExpressionFactory<T>
    {
        public Expression<Func<T, bool>> CreateExpression(IFilterNodeRelation<T> root)
        {
            var pExp = Expression.Parameter(typeof(T), "_40W");
            var leftExp = root.Left.CreateExpression().Unwrap(pExp);
            var rightExp = root.Right.CreateExpression().Unwrap(pExp);
            Expression bodyExp = root.Relation switch
            {
                "AndAlso" => Expression.AndAlso(leftExp, rightExp),
                "OrElse" => Expression.OrElse(leftExp, rightExp),
                _ => throw new RelationNotFoundException(root.Relation)
            };

            var re = Expression.Lambda<Func<T, bool>>(bodyExp, pExp);
            return re;
        }
    }
}
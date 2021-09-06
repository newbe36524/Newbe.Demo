using System;
using System.Linq.Expressions;

namespace Newbe.ExpressionsTests.FilterFactory.Impl
{
    public class FilterNodeRelation<T> : IFilterNodeRelation<T>
    {
        public FilterNodeRelation(IFilterNode<T> left, IFilterNode<T> right, string relation)
        {
            Left = left;
            Right = right;
            Relation = relation;
        }

        public IFilterNode<T> Left { get; }
        public IFilterNode<T> Right { get; }
        public string Relation { get; set; }

        public Expression<Func<T, bool>> CreateExpression()
        {
            var filterExpressionFactory = new FilterExpressionFactory<T>();
            var expression = filterExpressionFactory.CreateExpression(this);
            return expression;
        }
    }
}